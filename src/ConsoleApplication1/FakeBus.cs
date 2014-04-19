using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    public class BusFactory
    {
        private readonly Configuration _configuration;
        private readonly Dictionary<Type, Delegate> _dict = new Dictionary<Type, Delegate>();

        public BusFactory()
        {
            _configuration = new Configuration(_dict);
        }

        public void Configure(Action<Configuration> cfg)
        {
            cfg(_configuration);
        }

        public Bus Create()
        {
            return new Bus(_dict);
        }
    }

    public class Configuration
    {
        private readonly IDictionary<Type, Delegate> _dict;

        internal Configuration(IDictionary<Type, Delegate> dict)
        {
            _dict = dict;
        }

        public void Subscribe<TMessage>(Action<TMessage> handler)
        {
            _dict.Add(typeof (TMessage), handler);
        }
    }

    public class Bus
    {
        private readonly IDictionary<Type, Delegate> _mapDelegates;

        internal Bus(IDictionary<Type, Delegate> mapDelegates)
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
