using System;

namespace ConsoleApplication1
{
    public class CommandA
    {
    }
    public class CommandB
    {
    }
    public class CommandC
    {
    }
    public class CommandD
    {
    }
    

    public class CommandAHandler : IHandler<CommandA>
    {
        public void Handle(CommandA command)
        {
            Console.WriteLine("CommandAHandler: I handled a CommandA instance!");
        }
    }

    public class CommandBHandler : IHandler<CommandB>
    {
        public void Handle(CommandB command)
        {
            Console.WriteLine("CommandBHandler: I handled a CommandB instance!");
        }
    }

    public class DuploHandler : IHandler<CommandC>, IHandler<CommandD>
    {
        public void Handle(CommandC command)
        {
            Console.WriteLine("DuploHandler: I handled a CommandC instance!");
        }

        public void Handle(CommandD command)
        {
            Console.WriteLine("DuploHandler: I handled a CommandD instance!");
        }
    }

    
}
