using System;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Continuations;
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

        public FubuContinuation Index()
        {
            return FubuContinuation.RedirectTo("_fubu");

            //return "The Color in the config file is " + _settings.Color;
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
            return FubuApplication.DefaultPolicies().StructureMap();
        }


    }
}