using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Diagnostics.Configuration
{
    public class PartialActionSource : IActionSource
    {
        public IEnumerable<ActionCall> FindActions(TypePool types)
        {
            throw new NotImplementedException();
        }
    }
}