using System;
using FubuMVC.Core;
using System.Linq;
using System.Collections.Generic;

namespace Fubu.Applications
{
    [Serializable]
    public class ApplicationStartResponse
    {
        public ApplicationStartStatus Status { get; set; }
        public string ErrorMessage { get; set; }
        public string ApplicationSourceName { get; set; }
        public string[] ApplicationSourceTypes { get; set; }
        public string[] BottleDirectories { get; set; }

        public void WriteReport(ApplicationSettings settings)
        {
            switch (Status)
            {
                case ApplicationStartStatus.Started:
                    Console.WriteLine("Successfully started the application at " + settings.PhysicalPath);
                    Console.WriteLine("Using {0} as the application source", ApplicationSourceName);
                    break;

                case ApplicationStartStatus.CouldNotResolveApplicationSource:
                    Console.WriteLine("Could not determine an application source and none was provided");
                    
                    if (ApplicationSourceTypes.Any())
                    {
                        Console.WriteLine("Found:");

                        ApplicationSourceTypes.Each(x => Console.WriteLine(" * " + x));
                    }
                    else
                    {
                        Console.WriteLine("No IApplicationSource types were found in the assemblies in this folder");
                    }
                    break;



                case ApplicationStartStatus.ApplicationSourceFailure:
                    Console.WriteLine("Failed to start the application at " + settings.PhysicalPath);
                    Console.WriteLine(ErrorMessage);
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();

                    throw new ApplicationException("Loading the application failed!");


            }

            Console.WriteLine();
        }
    }
}