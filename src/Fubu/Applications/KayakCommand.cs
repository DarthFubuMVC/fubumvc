using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.CommandLine;
using FubuMVC.Core;

namespace Fubu.Applications
{
    public class KayakInput
    {
        public KayakInput()
        {
            Location = ".".ToFullPath();
            PortFlag = 5500;
        }

        public string Location { get; set; }
        public string UrlFlag { get; set; }
        public int PortFlag { get; set; }
    }

    /*
     * Like a way to create new project files
     * 
     * 
     * 
     */

    public class KayakCommand : FubuCommand<KayakInput>
    {
        public override bool Execute(KayakInput input)
        {
            var settings = FindSettings(input);
            if (settings == null) return false;


            settings.Port = input.PortFlag;
            var domain = new KayakApplicationDomain();
            var response = domain.Start(settings);

            response.WriteReport(settings);

            return true;
        }

        public ApplicationSettings FindSettings(KayakInput input)
        {
            var system = new FileSystem();
            if (system.IsFile(input.Location))
            {
                Console.WriteLine("Reading application settings from " + input.Location);
                return ApplicationSettings.Read(input.Location);
            }


            var files = system.FindFiles(input.Location, new FileSet{
                Include = "*.application.config"
            });


            if (!files.Any())
            {
                Console.WriteLine(
                    "Did not find any *.application.config file, \nwill try to determine the application source by scanning assemblies");
                return new ApplicationSettings{
                    PhysicalPath = input.Location,
                    ParentFolder = input.Location
                };
            }


            if (files.Count() == 1)
            {
                var location = files.Single();
                Console.WriteLine("Reading application settings from " + location);
                return ApplicationSettings.Read(input.Location);
            }


            Console.WriteLine("Found multiple *.application.config files, cannot determine which to use:");
            files.Each(x => Console.WriteLine(" - " + x));

            return null;
        }
    }
}