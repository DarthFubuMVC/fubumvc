using FubuMVC.Core;
using FubuMVC.Core.Registration;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests
{
    
    public class FubuRegistryTester
    {
        [Fact]
        public void what_is_the_order()
        {
            new FubuRegistry();
        }

        [Fact]
        public void policies_are_only_registered_once()
        {
            FakePolicy.Count = 0;

            BehaviorGraph.BuildFrom(x =>
            {
                x.Policies.Local.Add<FakePolicy>();
                x.Policies.Local.Add<FakePolicy>();
                x.Policies.Local.Add<FakePolicy>();
                x.Policies.Local.Add<FakePolicy>();
                x.Policies.Local.Add<FakePolicy>();
                x.Policies.Local.Add<FakePolicy>();
            });

            FakePolicy.Count.ShouldBe(1);
        }

        [Fact]
        public void registries_may_only_be_registered_once()
        {
            FakeIncludeRegistry.Count = 0;

            BehaviorGraph.BuildFrom(x =>
            {
                x.Import<FakeIncludeRegistry>(string.Empty);
                x.Import<FakeIncludeRegistry>(string.Empty);
                x.Import<FakeIncludeRegistry>(string.Empty);
                x.Import<FakeIncludeRegistry>(string.Empty);
            });

            FakeIncludeRegistry.Count.ShouldBe(1);
        }

        [Fact]
        public void alter_settings_modifies_settings_object()
        {
            var registry = new FubuRegistry(r => r.AlterSettings<SettingsObject>(so => so.Touched = true));
            
            BehaviorGraph.BuildFrom(registry)
                .Settings.Get<SettingsObject>().Touched.ShouldBeTrue();
        }

        [Fact]
        public void replace_settings()
        {
            var settings1 = new SettingsObject();

            var registry = new FubuRegistry();
            registry.ReplaceSettings(settings1);

            BehaviorGraph.BuildFrom(registry).Settings.Get<SettingsObject>()
                .ShouldBeTheSameAs(settings1);


        }
    }

    public class SettingsObject
    {
        public bool Touched { get; set; }
    }

    public class FakeIncludeRegistry : FubuRegistry
    {
        public static int Count;

        public FakeIncludeRegistry()
        {
            Configure(x => Count++);
        }
    }

    public class FakePolicy : IConfigurationAction
    {
        public static int Count;

        public void Configure(BehaviorGraph graph)
        {
            Count++;
        }
    }
}