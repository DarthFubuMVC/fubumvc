using System.Diagnostics;
using Bottles.Diagnostics;
using FubuMVC.Core.Assets;
using NUnit.Framework;
using FubuTestingSupport;
using System.Collections.Generic;

namespace FubuMVC.Tests.Assets
{
    [TestFixture]
    public class AssetDependencyFinderCacheTester
    {
        [Test]
        public void find_dependencies()
        {
            var graph = AssetGraph.Build(x =>
            {
                x.Dependency("a.js", "a-dep1.js");
                x.Dependency("a.js", "a-dep2.js");
                x.Dependency("b.js", "c.js");
            });

            var cache = new AssetDependencyFinderCache(graph);

            cache.CompileDependenciesAndOrder(new string[]{"b.js", "a.js"})
                .ShouldHaveTheSameElementsAs("a-dep1.js", "a-dep2.js", "c.js", "a.js", "b.js");

            cache.CompileDependenciesAndOrder(new string[] { "b.js", "a.js" })
                .ShouldHaveTheSameElementsAs("a-dep1.js", "a-dep2.js", "c.js", "a.js", "b.js");

        }
    }
}