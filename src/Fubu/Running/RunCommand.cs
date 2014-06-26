using System;
using FubuCore.CommandLine;
using FubuCore;
using FubuMVC.Core.Packaging;

namespace Fubu.Running
{
    [CommandDescription("Run a fubumvc application hosted in Katana")]
    public class RunCommand : FubuCommand<ApplicationRequest>
    {
        private RemoteApplication _application;


        public override bool Execute(ApplicationRequest input)
        {
            if (input.ExplodeOnlyFlag)
            {
                var bottleFolder = input.DirectoryFlag.AppendPath(FubuMvcPackageFacility.FubuContentFolder);
                ConsoleWriter.Write("Deleting the existing bottle content at " + bottleFolder);
                new FileSystem().DeleteDirectory(bottleFolder);
            }

            _application = new RemoteApplication();
            _application.Start(input);

            if (_application.Failed)
            {
                ConsoleWriter.Write(ConsoleColor.Yellow, "Application failed to start, exiting");
                return false;
            }

            if (input.ExplodeOnlyFlag)
            {
                ConsoleWriter.Write(ConsoleColor.Green, "Successfully exploded all the bottle content for the application at " + input.DirectoryFlag);
            }

            if (input.TemplatesFlag)
            {
                _application.GenerateTemplates();
            }

            if (!input.ShouldRunApp())
            {
                _application.Shutdown();
                return true;
            }

            tellUsersWhatToDo();
            ConsoleKeyInfo key = Console.ReadKey();
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