using System;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Assets;
using Shouldly;
using Xunit;

namespace FubuMVC.IntegrationTesting.Assets.FindingAndResolving
{
    
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
                AlterSettings<AssetSettings>(x =>
                {
                    x.Aliases.Add("foo", "Foo.js");
                    x.Aliases.Add("bar", "Foo.js");
                });
            }
        }

        [Fact]
        public void asset_graph_stores_aliases()
        {
            Assets.FindAsset("bar").Url.ShouldBe("Folder1/Foo.js");
        }

        [Fact]
        public void alias_is_not_found_sad_path()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(
                () => { AllAssets.As<AssetGraph>().StoreAlias("bar", "nonexistent.js"); })
                .Message.ShouldContain("No asset file named 'nonexistent.js' can be found");
        }

        [Fact]
        public void aliases_are_applied_at_bootstrapping_time()
        {
            Assets.FindAsset("foo").Url.ShouldBe("Folder1/Foo.js");
        }
    }
}