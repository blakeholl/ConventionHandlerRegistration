using System;

namespace ConsoleApplication1
{
    public interface IHandlerByConvention
    {
        Type CommandType { get; }
        Type CommandHandlerType { get; }
        Type InterfaceType { get; }
    }
}