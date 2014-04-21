using System;
using System.Reflection;
using Ninject;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var kernel = new StandardKernel();
            var busFactory = new FakeBusFactory();

            MassTransitHelper.ConfigureBusFactory(busFactory, Assembly.GetExecutingAssembly(), kernel);

            var bus = busFactory.Create();
            bus.Send(new CommandA());
            bus.Send(new CommandB());
            bus.Send(new CommandC());
            bus.Send(new CommandD());
            Console.Read();
        }
    }
}