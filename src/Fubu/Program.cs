using System;
using FubuCore.CommandLine;

namespace Fubu
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            try
            {
                var factory = new CommandFactory();
                factory.RegisterCommands(typeof (IFubuCommand).Assembly);
                factory.RegisterCommands(typeof (Program).Assembly);

                var executor = new CommandExecutor(factory);
                executor.Execute(args);
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR: " + ex);
                Console.ResetColor();
                return 1;
            }
            return 0;
        }
    }
}