using System;
using System.Collections.Generic;
using FubuCore.Configuration;
using FubuMVC.Core;
using Shouldly;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.Tests.StructureMap.Settings
{
    [TestFixture]
    public class SettingIntegrationTester
    {
        private FubuRegistry registry;
        private Lazy<IContainer> container;

        [SetUp]
        public void SetUp()
        {
            registry = new FubuRegistry();
            container = new Lazy<IContainer>(() => {
                var c = new Container(x => {
                    x.For<ISettingsProvider>().Use<SettingsProvider>();

                    x.For<ISettingsSource>().Add<FakeSettingsData>();
                    x.For<ISettingsSource>().Add(new AppSettingsSettingSource(SettingCategory.core));
                });

                registry.StructureMap(c);

                registry.ToRuntime();

                return c;
            });

        }

        public FooSettings TheResultingSettings
        {
            get { return container.Value.GetInstance<FooSettings>(); }
        }

        [Test]
        public void include_explicitly()
        {
            TheResultingSettings.Name.ShouldBe("Max");
            TheResultingSettings.Age.ShouldBe(9);
        }

        [Test]
        public void include_by_settings_convention_in_the_application_assembly()
        {
            container.Value.GetInstance<BarSettings>().Direction.ShouldBe("North");
            TheResultingSettings.Name.ShouldBe("Max");
            TheResultingSettings.Age.ShouldBe(9);
        }

        [Test]
        public void include_by_settings_convention_by_picking_the_assembly()
        {
            container.Value.GetInstance<BarSettings>().Direction.ShouldBe("North");
            TheResultingSettings.Name.ShouldBe("Max");
            TheResultingSettings.Age.ShouldBe(9);
        }

        [Test]
        public void do_not_override_a_setting_class_that_is_configured_inside_the_fubu_registry()
        {
            registry.Services.ReplaceService(new BarSettings {Direction = "West"});

            container.Value.GetInstance<BarSettings>().Direction.ShouldBe("West");
        }
    }


    public class FooSettings
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class BarSettings
    {
        public string Direction { get; set; }
    }

    public class FakeSettingsData : ISettingsSource
    {
        public IEnumerable<SettingsData> FindSettingData()
        {
            var data = new SettingsData();
            data.Child("FooSettings").Set("Name", "Max");
            data.Child("FooSettings").Set("Age", "9");
            data.Child("BarSettings").Set("Direction", "North");

            yield return data;
        }
    }
}