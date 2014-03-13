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
            useNamespaces.Namespaces.Each(x => Debug.WriteLine(x));

            useNamespaces.Namespaces.ShouldHaveTheSameElementsAs(new[]
            { 
                typeof(VirtualPathUtility).Namespace,
                typeof(string).Namespace,
                typeof(FileSet).Namespace,
                typeof(ParallelQuery).Namespace,
                typeof(HtmlTag).Namespace,
                "Foo",
                "Bar",
                "FubuMVC.Tests.Docs.Introduction.Overview",
                "FubuMVC.Tests.Http.Hosting",
"FubuMVC.Tests.Registration",
"FubuMVC.Core.Continuations",
"FubuMVC.Tests.Registration.Conventions",
"FubuMVC.Core.Resources.PathBased",
"FubuMVC.Tests.Registration.Policies",
"FubuMVC.Tests.Urls",

            });
        }
    }
}