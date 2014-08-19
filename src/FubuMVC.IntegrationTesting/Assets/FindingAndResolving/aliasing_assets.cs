using System;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core;
using FubuMVC.Core.Assets;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Assets.FindingAndResolving
{
    [TestFixture]
    public class aliasing_assets : AssetIntegrationContext
    {
        public aliasing_assets()
        {
            File("Folder1/Foo.js");
            File("Folder1/Bar.js");
        }

        public class MyRegistry : FubuRegistry
        {
            public MyRegistry()
            {
                AlterSettings<AssetSettings>(x => {
                    x.Aliases.Add("foo", "Foo.js");
                    x.Aliases.Add("bar", "Foo.js");
                });
            }
        }

        [Test]
        public void asset_graph_stores_aliases()
        {
            Assets.FindAsset("bar").Url.ShouldEqual("Folder1/Foo.js");
        }

        [Test]
        public void alias_is_not_found_sad_path()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() => {
                AllAssets.As<AssetGraph>().StoreAlias("bar", "nonexistent.js");
            }).Message.ShouldContain("No asset file named 'nonexistent.js' can be found");
        }

        [Test]
        public void aliases_are_applied_at_bootstrapping_time()
        {
            Assets.FindAsset("foo").Url.ShouldEqual("Folder1/Foo.js");
        }
    }
}