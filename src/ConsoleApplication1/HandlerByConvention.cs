using System;

namespace ConsoleApplication1
{
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