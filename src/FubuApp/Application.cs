using System;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Packaging;
using FubuMVC.StructureMap;
using FubuMVC.StructureMap.Settings;
using StructureMap;

namespace FubuApp
{
    public class ColorSettings
    {
        public string Color { get; set; }
    }

    public class HomeEndpoint
    {
        private readonly ColorSettings _settings;
        public static DateTime First = DateTime.Now;

        public HomeEndpoint(ColorSettings settings)
        {
            _settings = settings;
        }

        public string Index()
        {
            return "The Color in the config file is " + _settings.Color;
        }

        public string get_file()
        {
            var filename = FubuMvcPackageFacility.GetApplicationPath().AppendPath("Something.spark");
            return new FileSystem().ReadStringFromFile(filename);
        }

        public string get_time()
        {
            return "AppDomain Startup Time = " + First.ToLongTimeString() + "\n\nApplication Startup Time = " + FubuMvcPackageFacility.Restarted.Value.ToLongTimeString();
        }

    }

    public class SampleApplication : IApplicationSource
    {
        

        public FubuApplication BuildApplication()
        {
            //throw new NotImplementedException("You suck!");

            var registry = new FubuRegistry();
            registry.AlterSettings<ConfigurationSettings>(x => {
                x.Include<ColorSettings>();
            });

            return FubuApplication.For(registry).StructureMap(new Container());
        }


    }
}