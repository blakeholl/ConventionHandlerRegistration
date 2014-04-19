using System;
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
            var busFactory = new BusFactory();

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
                var configureExpression = configureMethodInfo.Invoke(null, new[] {handlerExpression});

                var ce = (dynamic) configureExpression;
                busFactory.Configure(cfg => ce.Compile()(cfg));
            }

            var bus = busFactory.Create();
            bus.Send(new CommandA());
            bus.Send(new CommandB());
            bus.Send(new CommandC());
            bus.Send(new CommandD());
            Console.Read();
        }
    }

    public interface IHandlerByConvention
    {
        Type CommandType { get; }
        Type CommandHandlerType { get; }
        Type InterfaceType { get; }
    }

    public class HandlerByConvention : IHandlerByConvention
    {
        internal HandlerByConvention(Type commandType, Type commandHandlerType, Type interfaceType)
        {
            InterfaceType = interfaceType;
            CommandHandlerType = commandHandlerType;
            CommandType = commandType;
        }

        public Type CommandType { get; private set; }
        public Type CommandHandlerType { get; private set; }
        public Type InterfaceType { get; private set; }
    }
}