using System;
using System.Collections.Generic;
using FubuMVC.Core;
using FubuMVC.Core.StructureMap;
using StructureMap;

namespace KayakTestApplication
{
    public class KayakApplication : IApplicationSource
    {
        public FubuApplication BuildApplication(string directory)
        {
            return FubuApplication
                .For<KayakRegistry>()
                ;
        }
    }

    public class KayakRegistry : FubuRegistry
    {
        public KayakRegistry()
        {
            Actions.IncludeClassesSuffixedWithController();
            Actions.IncludeType<NameModelEndpoint>();
        }
    }

    public class NameModelEndpoint
    {
        public NameModel get_name_model(NameModel input)
        {
            return input;
        }
    }

    public class NameModel
    {
        public string Name { get; set; }
    }

    public class HomeEndpoint
    {
        public string Index()
        {
            return "Hello, it's " + DateTime.Now;
        }

        public NameModel get_say_Name(NameModel model)
        {
            return model;
        }

        public Dictionary<string, object> post_name(NameModel model)
        {
            return new Dictionary<string, object>{{"name", model.Name}};
        }
    }
}