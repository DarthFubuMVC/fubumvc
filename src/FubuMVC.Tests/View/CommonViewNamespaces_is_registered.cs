using System.Linq;
using System.Web;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.View;
using Shouldly;
using HtmlTags;
using Xunit;
using StructureMap;

namespace FubuMVC.Tests.View
{
    
    public class CommonViewNamespaces_is_registered
    {
        [Fact]
        public void is_registered()
        {
            var registry = new FubuRegistry();
            registry.AlterSettings<CommonViewNamespaces>(x =>
            {
                x.Add("Foo");
                x.Add("Bar");
            });

            using (var runtime = registry.ToRuntime())
            {
                var container = runtime.Get<IContainer>();

                var useNamespaces = container.GetInstance<CommonViewNamespaces>();

                useNamespaces.Namespaces.ShouldContain(typeof (VirtualPathUtility).Namespace);
                useNamespaces.Namespaces.ShouldContain(typeof (string).Namespace);
                useNamespaces.Namespaces.ShouldContain(typeof (FileSet).Namespace);
                useNamespaces.Namespaces.ShouldContain(typeof (ParallelQuery).Namespace);
                useNamespaces.Namespaces.ShouldContain(typeof (HtmlTag).Namespace);
                useNamespaces.Namespaces.ShouldContain("FubuMVC.Tests.Http.Hosting");
                useNamespaces.Namespaces.ShouldContain("Foo");
                useNamespaces.Namespaces.ShouldContain("Bar");
            }
        }
    }
}