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

        public static ApplicationSettings FindSettings(KayakInput input)
        {
            if (input.Location.IsEmpty())
            {
                return new ApplicationSettings{
                    PhysicalPath = ".".ToFullPath(),
                    ParentFolder = ".".ToFullPath()
                };
            }

            var system = new FileSystem();
            if (system.FileExists(input.Location) && system.IsFile(input.Location))
            {
                Console.WriteLine("Reading application settings from " + input.Location);
                return ApplicationSettings.Read(input.Location);
            }

            if (system.DirectoryExists(input.Location))
            {
                return findFromDirectory(system, input);
            }

            return findByName(system, input);
        }

        private static ApplicationSettings findByName(FileSystem system, KayakInput input)
        {

            var files = system.FindFiles(".".ToFullPath(), ApplicationSettings.FileSearch(input.Location));
            if (!files.Any())
            {
                Console.WriteLine("Could not find any matching *.application.config files");
                return null;
            }

            if (files.Count() == 1)
            {
                var location = files.Single();
                Console.WriteLine("Using file " + location);
                return ApplicationSettings.Read(location);
            }

            Console.WriteLine("Found multiple *.application.settings files");
            files.Each(x => Console.WriteLine(" - " + x));

            return null;
        }

        private static ApplicationSettings findFromDirectory(FileSystem system, KayakInput input)
        {
            var files = system.FindFiles(input.Location, ApplicationSettings.FileSearch());


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
                return ApplicationSettings.Read(location);
            }


            Console.WriteLine("Found multiple *.application.config files, cannot determine which to use:");
            files.Each(x => Console.WriteLine(" - " + x));

            return null;
        }
    }
}