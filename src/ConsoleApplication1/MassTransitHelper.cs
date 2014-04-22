using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using MassTransit;
using MassTransit.BusConfigurators;
using MassTransit.SubscriptionConfigurators;
using Ninject;
using Ninject.Parameters;
using Ninject.Syntax;
using RabbitMQ.Client.Impl;

namespace ConsoleApplication1
{
    internal static class MassTransitHelper
    {
        // this is just some clever code to help auto register my message handlers.  This is not intended for production use

        private static IEnumerable<IHandlerByConvention> GetHandlersByConvention(Assembly assembly)
        {
            return assembly.GetTypes()
                .SelectMany(
                    ht =>
                        ht.GetInterfaces()
                            .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof (IHandler<>)),
                    (ht, i) => new HandlerByConvention(i.GetGenericArguments().First(), ht, i))
                .ToList();
        }

        private static Expression<Func<T>> ContainerHandlerFactoryExpression<T>(IKernel kernel)
        {
            var emptyNinjectParameters = Expression.Constant(new IParameter[] {});
            var containerExpression = Expression.Constant(kernel);
            var methodInfo = typeof (ResolutionExtensions).GetMethod("Get", new[] {typeof(IResolutionRoot), typeof (IParameter[])}).MakeGenericMethod(typeof(T));
            var call = Expression.Call(null, methodInfo, containerExpression, emptyNinjectParameters);
            return Expression.Lambda<Func<T>>(call);
        }

        private static Expression<Action<TCommand>> HandleCommandExpression<TCommand, THandler>(Expression<Func<THandler>> handlerFactory)
        {
            var factoryInvoker = Expression.Invoke(handlerFactory);
            var handleMi = typeof (THandler).GetMethod("Handle", new[] {typeof (TCommand)});
            var param = Expression.Parameter(typeof (TCommand), "command");
            var call = Expression.Call(factoryInvoker, handleMi, param);
            return Expression.Lambda<Action<TCommand>>(call, param);
        }

        private static Expression<Action<SubscriptionBusServiceConfigurator>> RegisterCommandHandlerExpression<TCommand>(
            Expression<Action<TCommand>> handleExpression)
        {
            Action<object> ignore = null;
            Expression<Action<CommandA>> asd = a => HandlerSubscriptionExtensions.Handler(null, ignore);
            var mce = asd.Body as MethodCallExpression;
            var configMi = mce.Method.GetGenericMethodDefinition().MakeGenericMethod(new[] { typeof(TCommand) });
            var param = Expression.Parameter(typeof (SubscriptionBusServiceConfigurator), "cfg");
            var call = Expression.Call(null, configMi, param, handleExpression);
            return Expression.Lambda<Action<SubscriptionBusServiceConfigurator>>(call, param);
        }

        public static void ConfigureBusFactory(
            ServiceBusConfigurator configurator,
            Assembly assembly,
            IKernel kernel)
        {
            var handlers = GetHandlersByConvention(assembly);

            foreach (var handler in handlers)
            {
                var factoryMethodInfo = typeof(MassTransitHelper)
                    .GetMethod("ContainerHandlerFactoryExpression", BindingFlags.Static | BindingFlags.NonPublic)
                    .MakeGenericMethod(handler.CommandHandlerType);

                var handlerMethodInfo = typeof(MassTransitHelper)
                    .GetMethod("HandleCommandExpression", BindingFlags.Static | BindingFlags.NonPublic)
                    .MakeGenericMethod(new[] { handler.CommandType, handler.CommandHandlerType });

                var configureMethodInfo = typeof(MassTransitHelper)
                    .GetMethod("RegisterCommandHandlerExpression", BindingFlags.Static | BindingFlags.NonPublic)
                    .MakeGenericMethod(new[] { handler.CommandType });

                var factoryExpression = factoryMethodInfo.Invoke(null, new object[] { kernel });
                var handlerExpression = handlerMethodInfo.Invoke(null, new[] { factoryExpression });
                var configureExpression = (Expression<Action<SubscriptionBusServiceConfigurator>>)configureMethodInfo.Invoke(null, new[] { handlerExpression });

                configurator.Subscribe(configureExpression.Compile());
            }
        }

        

    }
}