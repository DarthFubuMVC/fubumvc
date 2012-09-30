using System.Collections.Generic;
using Bottles;
using Bottles.Diagnostics;
using FubuMVC.Core.Bootstrapping;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.ObjectGraph;
using HtmlTags.Conventions;

namespace FubuMVC.Core.UI
{
    public class HtmlConventionsActivator : IActivator
    {
        private readonly BehaviorGraph _graph;
        private readonly IContainerFacility _facility;

        public HtmlConventionsActivator(BehaviorGraph graph, IContainerFacility facility)
        {
            _graph = graph;
            _facility = facility;
        }

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            var library = _graph.Settings.Get<HtmlConventionLibrary>();
            library.Import(new DefaultHtmlConventions().Library);

            _facility.Register(typeof(HtmlConventionLibrary), ObjectDef.ForValue(library));
        }
    }
}