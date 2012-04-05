using System;
using System.Collections.Generic;
using Bottles;
using Bottles.Diagnostics;
using FubuMVC.Core.Bootstrapping;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.UI.Configuration;
using FubuMVC.Core.UI.Tags;

namespace FubuMVC.Core.UI
{
    public class HtmlConventionsActivator : IActivator
    {
        private readonly IEnumerable<HtmlConventionRegistry> _conventions;
        private readonly IContainerFacility _container;

        public HtmlConventionsActivator(IEnumerable<HtmlConventionRegistry> conventions, IContainerFacility container)
        {
            _conventions = conventions;
            _container = container;
        }

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            var library = new TagProfileLibrary();

            _conventions.Each(library.ImportRegistry);

            library.ImportRegistry(new DefaultHtmlConventions());
            library.Seal();

            _container.Register(typeof(TagProfileLibrary), ObjectDef.ForValue(library));
        }
    }
}