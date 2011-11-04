using System;
using FubuCore.CommandLine;
using Serenity.Jasmine;

namespace SerenityRunner
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            var success = false;

            try
            {
                var factory = new CommandFactory();
                factory.RegisterCommands(typeof (Program).Assembly);
                factory.RegisterCommands(typeof (JasminePages).Assembly);


                var executor = new CommandExecutor(factory);
                success = executor.Execute(args);
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
            return success ? 0 : 1;
        }
    }
}