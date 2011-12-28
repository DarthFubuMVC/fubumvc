using System;
using System.Collections.Generic;
using FubuMVC.Core;
using FubuMVC.StructureMap;
using StructureMap;
using FubuMVC.Spark;

namespace KayakTestApplication
{
    public class KayakApplication : IApplicationSource
    {
        public FubuApplication BuildApplication()
        {
            return FubuApplication
                .For<KayakRegistry>()
                .StructureMap(new Container());
        }
    }

    public class KayakRegistry : FubuRegistry
    {
        public KayakRegistry()
        {
            Routes.HomeIs<SayHelloController>(x => x.Hello());

            Actions.IncludeClassesSuffixedWithController();
            this.UseSpark();

            IncludeDiagnostics(true);

            Views.TryToAttachWithDefaultConventions();
        }
    }

    public class NameModel
    {
        public string Name { get; set; }
    }

    public class SayHelloController
    {
        public string Hello()
        {
            return "Hello, it's " + DateTime.Now;
        }

        public NameModel get_say_Name(NameModel model)
        {
            return model;
        }

        public IDictionary<string, object> post_name(NameModel model)
        {
            return new Dictionary<string, object>{{"name", model.Name}};
        }
    }
}