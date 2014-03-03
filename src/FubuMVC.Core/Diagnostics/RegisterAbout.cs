using System.Collections.Generic;
using System.Reflection;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Diagnostics
{
    [Title("Register the _about endpoint")]
    public class RegisterAbout : IActionSource
    {
        public IEnumerable<ActionCall> FindActions(Assembly applicationAssembly)
        {
            return new []
            {
                ActionCall.For<AboutDiagnostics>(x => x.get__about()),
                ActionCall.For<AboutDiagnostics>(x => x.get__loaded())
            };
        }
    }
}