using System;
using FubuMVC.Core;
using FubuMVC.StructureMap;
using StructureMap;

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
            Route("").Calls<SayHelloController>(x => x.Hello());
        }
    }

    public class SayHelloController
    {
        public string Hello()
        {
            return "Hello!";
        }
    }
}