using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    public interface IContainer
    {
        T Get<T>();
    }

    public class FakeContainer : IContainer
    {
        public T Get<T>()
        {
            Console.WriteLine("FakeContainer: Created a new [{0}] instance", typeof (T).Name);
            return Activator.CreateInstance<T>();
        }
    }
}
