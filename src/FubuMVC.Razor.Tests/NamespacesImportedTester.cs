using System.Collections.Generic;
using System.Diagnostics;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Razor.Tests
{
    [TestFixture]
    public class NamespacesImportedTester
    {
        [Test]
        public void default_namespaces_are_set_including_anything_from_CommonViewNamespaces()
        {
            var registry = new FubuRegistry();
            registry.Import<RazorViewFacility>();
            registry.AlterSettings<CommonViewNamespaces>(x =>
            {
                x.Add("Foo");
                x.Add("Bar");
            });

            var graph = BehaviorGraph.BuildFrom(registry);
            var useNamespaces =
                graph.Services.DefaultServiceFor<CommonViewNamespaces>().Value.As<CommonViewNamespaces>();

            useNamespaces.Namespaces.Each(x => Debug.WriteLine(x));

            useNamespaces.Namespaces.ShouldHaveTheSameElementsAs(new[]
            {
                "System.Web",
                "System",
                "FubuCore",
                "System.Linq",
                "HtmlTags",
                "FubuMVC.Core.View",
                "FubuMVC.Razor",
                "FubuMVC.Core.Runtime",
                "Foo",
                "Bar"
            });
        }
    }
}