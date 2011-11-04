using System;
using FubuMVC.Core;
using FubuMVC.StructureMap;
using StructureMap;

namespace FubuTestApplication
{
    public class OwinApplication : FubuRegistry, IApplicationSource
    {
        public OwinApplication()
        {
            IncludeDiagnostics(true);
            Actions.IncludeType<OwinActions>();

            Routes.HomeIs<OwinActions>(x => x.Home());
        }

        public FubuApplication BuildApplication()
        {
            return FubuApplication.For(this).StructureMap(new Container());
        }
    }

    public class OwinActions
    {
        public string get_say_hello()
        {
            return "Hello, world!";
        }

        public string Home()
        {
            return "This is home";
        }
    }
}