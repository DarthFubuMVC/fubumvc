using System;
using FubuCore.CommandLine;

namespace Fubu.Running
{
    [CommandDescription("Run a fubumvc application hosted in Katana")]
    public class RunCommand : FubuCommand<ApplicationRequest>
    {
        private RemoteApplication _application;


        public override bool Execute(ApplicationRequest input)
        {
            _application = new RemoteApplication();
            _application.Start(input);

            if (_application.Failed)
            {
                ConsoleWriter.Write(ConsoleColor.Yellow, "Application failed to start, exiting");
                return false;
            }

            tellUsersWhatToDo();
            var key = Console.ReadKey();
            while (key.Key != ConsoleKey.Q)
            {
                if (key.Key == ConsoleKey.R)
                {
                    Console.WriteLine();
                    Console.WriteLine();

                    _application.RecycleAppDomain();

                    tellUsersWhatToDo();
                    key = Console.ReadKey();
                }
            }

            _application.Shutdown();

            return true;
        }

        private static void tellUsersWhatToDo()
        {
            Console.WriteLine("Press 'q' to quit or 'r' to recycle the application");
        }
    }
}