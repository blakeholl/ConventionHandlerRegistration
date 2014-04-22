using System;
using System.Reflection;
using MassTransit;
using Ninject;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var kernel = new StandardKernel();
            var bus = ServiceBusFactory.New(cfg =>
            {
                cfg.ReceiveFrom("loopback://localhost/queue");
                MassTransitHelper.ConfigureBusFactory(cfg, Assembly.GetExecutingAssembly(), kernel);
            });

            bus.Endpoint.Send(new CommandA());
            bus.Endpoint.Send(new CommandB());
            bus.Endpoint.Send(new CommandC());
            bus.Endpoint.Send(new CommandD());
            Console.Read();
        }
    }
}