using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    public class FakeBusFactory
    {
        private readonly IConfiguration _fakeConfiguration;
        private readonly Dictionary<Type, Delegate> _dict = new Dictionary<Type, Delegate>();

        public FakeBusFactory()
        {
            _fakeConfiguration = new FakeConfiguration(_dict);
        }

        public void Configure(Action<IConfiguration> cfg)
        {
            cfg(_fakeConfiguration);
        }

        public IBus Create()
        {
            return new FakeBus(_dict);
        }
    }

    public interface IConfiguration
    {
        void Subscribe<TMessage>(Action<TMessage> handler);
    }

    public class FakeConfiguration : IConfiguration
    {
        private readonly IDictionary<Type, Delegate> _dict;

        internal FakeConfiguration(IDictionary<Type, Delegate> dict)
        {
            _dict = dict;
        }

        public void Subscribe<TMessage>(Action<TMessage> handler)
        {
            _dict.Add(typeof (TMessage), handler);
        }
    }

    public interface IBus
    {
        void Send<TMessage>(TMessage message);
    }

    public class FakeBus : IBus
    {
        private readonly IDictionary<Type, Delegate> _mapDelegates;

        internal FakeBus(IDictionary<Type, Delegate> mapDelegates)
        {
            _mapDelegates = mapDelegates;
        }

        public void Send<TMessage>(TMessage message)
        {
            Console.WriteLine("FakeBus sending a [{0}] command", typeof(TMessage).Name);
            dynamic del = _mapDelegates[typeof (TMessage)];
            del(message);
        }
    }
}
