using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ConsoleApplication1
{
    internal static class Helper
    {
        public static IEnumerable<IHandlerByConvention> GetHandlersByConvention(Assembly assembly)
        {
            return assembly.GetTypes()
                .SelectMany(
                    ht =>
                        ht.GetInterfaces()
                            .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof (IHandler<>)),
                    (ht, i) => new HandlerByConvention(i.GetGenericArguments().First(), ht, i))
                .ToList();
        }

        public static Expression<Func<T>> ContainerHandlerFactoryExpression<T>(IContainer container)
        {
            var containerExpression = Expression.Constant(container);
            var methodInfo = typeof(IContainer).GetMethod("Get").MakeGenericMethod(typeof(T));
            var call = Expression.Call(containerExpression, methodInfo);
            return Expression.Lambda<Func<T>>(call);
        }

        public static Expression<Action<TCommand>> HandleCommandExpression<TCommand, THandler>(Expression<Func<THandler>> handlerFactory)
        {
            var factoryInvoker = Expression.Invoke(handlerFactory);
            var handleMi = typeof (THandler).GetMethod("Handle", new[] {typeof (TCommand)});
            var param = Expression.Parameter(typeof (TCommand), "command");
            var call = Expression.Call(factoryInvoker, handleMi, param);
            return Expression.Lambda<Action<TCommand>>(call, param);
        }

        public static Expression<Action<IConfiguration>> RegisterCommandHandlerExpression<TCommand>(
            Expression<Action<TCommand>> handleExpression)
        {
            var param = Expression.Parameter(typeof (IConfiguration), "cfg");
            var configMi = typeof(IConfiguration).GetMethod("Subscribe").MakeGenericMethod(typeof(TCommand));
            var call = Expression.Call(param, configMi, handleExpression);
            return Expression.Lambda<Action<IConfiguration>>(call, param);
        }

    }
}