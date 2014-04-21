using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var handlers = Helper.GetHandlersByConvention(Assembly.GetExecutingAssembly());

            // these are fakes infrastructure components
            IContainer container = new FakeContainer();
            var busFactory = new FakeBusFactory();

            foreach (var handler in handlers)
            {
                var factoryMethodInfo = typeof (Helper)
                    .GetMethod("ContainerHandlerFactoryExpression")
                    .MakeGenericMethod(handler.CommandHandlerType);

                var handlerMethodInfo = typeof (Helper)
                    .GetMethod("HandleCommandExpression")
                    .MakeGenericMethod(new[] {handler.CommandType, handler.CommandHandlerType});

                var configureMethodInfo = typeof(Helper)
                    .GetMethod("RegisterCommandHandlerExpression")
                    .MakeGenericMethod(new[] { handler.CommandType });

                var factoryExpression = factoryMethodInfo.Invoke(null, new object[] { container });
                var handlerExpression = handlerMethodInfo.Invoke(null, new[] {factoryExpression});
                var configureExpression = (Expression<Action<IConfiguration>>) configureMethodInfo.Invoke(null, new[] { handlerExpression });

                busFactory.Configure(cfg => configureExpression.Compile()(cfg));
            }

            var bus = busFactory.Create();
            bus.Send(new CommandA());
            bus.Send(new CommandB());
            bus.Send(new CommandC());
            bus.Send(new CommandD());
            Console.Read();
        }
    }
}