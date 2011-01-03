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
                factory.RegisterCommands(typeof(IFubuCommand).Assembly);
                factory.RegisterCommands(typeof(Program).Assembly);

                var executor = new CommandExecutor(factory);
                executor.Execute(args);
            }
            catch (CommandFailureException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR: " + e.Message);
                Console.ResetColor();
                return 1;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR: " + ex);
                Console.ResetColor();
                return 1;
            }
            return 0;
        }
    }

    public class CommandFailureException : ApplicationException
    {
        public CommandFailureException(string message) : base(message)
        {
        }
    }
}