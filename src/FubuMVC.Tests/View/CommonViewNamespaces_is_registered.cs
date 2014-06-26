using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.Tests.View
{
    [TestFixture]
    public class CommonViewNamespaces_is_registered
    {
        [Test]
        public void is_registered()
        {
            var registry = new FubuRegistry();
            registry.AlterSettings<CommonViewNamespaces>(x =>
            {
                x.Add("Foo");
                x.Add("Bar");
            });

            var graph = BehaviorGraph.BuildFrom(registry);
            var useNamespaces = graph.Services.DefaultServiceFor<CommonViewNamespaces>().Value.As<CommonViewNamespaces>();

            useNamespaces.Namespaces.ShouldContain(typeof(VirtualPathUtility).Namespace);
            useNamespaces.Namespaces.ShouldContain(typeof(string).Namespace);
            useNamespaces.Namespaces.ShouldContain(typeof(FileSet).Namespace);
            useNamespaces.Namespaces.ShouldContain(typeof(ParallelQuery).Namespace);
            useNamespaces.Namespaces.ShouldContain(typeof(HtmlTag).Namespace);
            useNamespaces.Namespaces.ShouldContain("FubuMVC.Tests.Http.Hosting");
            useNamespaces.Namespaces.ShouldContain("Foo");
            useNamespaces.Namespaces.ShouldContain("Bar");
        }
    }
}