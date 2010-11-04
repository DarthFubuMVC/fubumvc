using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using FubuMVC.Core;
using FubuMVC.StructureMap.Bootstrap;

namespace FubuTestApplication
{
    public class Global : FubuStructureMapApplication
    {
        public override FubuRegistry GetMyRegistry()
        {
            return new FubuTestApplicationRegistry();
        }
    }

    public class FubuTestApplicationRegistry : FubuRegistry
    {
        public FubuTestApplicationRegistry()
        {
            IncludeDiagnostics(true);

            Applies.ToThisAssembly();
            Actions.IncludeTypesNamed(name => name.EndsWith("Controller"));
        }
    }
}
