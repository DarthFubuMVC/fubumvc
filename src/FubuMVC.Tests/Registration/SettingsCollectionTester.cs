using System;
using System.Threading;
using FubuMVC.Core.Registration;
using FubuMVC.Tests.UI;
using NUnit.Framework;
using FubuTestingSupport;
using Rhino.Mocks;

namespace FubuMVC.Tests.Registration
{
    [TestFixture]
    public class SettingsCollection_with_a_parent_Tester
    {
        private SettingsCollection theSettings;
        private SettingsCollection theParent;

        [SetUp]
        public void SetUp()
        {
            theParent = new SettingsCollection(null);
            theSettings = new SettingsCollection(theParent);
        }

        [Test]
        public void pull_defaults_from_the_parent_if_it_exists_if_it_is_missing_in_child()
        {
            theParent.Replace(new FakeSettings());

            theSettings.Get<FakeSettings>().ShouldBeTheSameAs(theParent.Get<FakeSettings>());
        }

        [Test]
        public void pull_defaults_from_parent_if_the_settings_is_marked_as_ApplicationLevel()
        {
            theParent.HasExplicit<AppSettings>().ShouldBeFalse();

            // should force the parent to build it now
            theSettings.Get<AppSettings>().ShouldBeTheSameAs(theParent.Get<AppSettings>());
        }

        [Test]
        public void child_value_superceeds_the_parent()
        {
            theParent.Replace(new FakeSettings());
            theSettings.Replace(new FakeSettings{Hometown = "Austin"});

            theSettings.Get<FakeSettings>().ShouldNotBeTheSameAs(theParent.Get<FakeSettings>());
            theSettings.Get<FakeSettings>().Hometown.ShouldEqual("Austin");
        }

        [Test]
        public void has_explicit_is_false_if_the_parent_has_it_but_the_child_does_not()
        {
            theSettings.HasExplicit<FakeSettings>().ShouldBeFalse();

            theParent.Replace(new FakeSettings());

            theSettings.HasExplicit<FakeSettings>().ShouldBeFalse();
        }

        [Test]
        public void has_explicit_is_true_if_the_child_has_it()
        {
            theSettings.Replace(new FakeSettings());

            theSettings.HasExplicit<FakeSettings>().ShouldBeTrue();
        }

        [Test]
        public void replace_is_isolated_from_the_parent()
        {
            theSettings.Replace(new FakeSettings());

            theSettings.Get<FakeSettings>().ShouldNotBeTheSameAs(theParent.Get<FakeSettings>());
        }

        [Test]
        public void alter_is_isolated()
        {
            theParent.Replace(new FakeSettings());

            theSettings.Alter<FakeSettings>(x => x.Hometown = "Austin");

            theParent.Get<FakeSettings>().Hometown.ShouldNotEqual("Austin");
            theSettings.Get<FakeSettings>().Hometown.ShouldEqual("Austin");
        }

        [Test]
        public void alter_will_use_the_parent_for_application_level_settings()
        {
            theSettings.Alter<AppSettings>(fake => fake.Name = "Shiner");

            theSettings.Get<AppSettings>().ShouldBeTheSameAs(theParent.Get<AppSettings>());
            theSettings.Get<AppSettings>().Name.ShouldEqual("Shiner");
        }
    }


    [TestFixture]
    public class SettingsCollection_without_a_parent_Tester
    {
        private SettingsCollection theSettings;

        [SetUp]
        public void SetUp()
        {
            theSettings = new SettingsCollection(null);
        }

        [Test]
        public void get_can_happily_create_the_default()
        {
            theSettings.Get<FakeSettings>().ShouldEqual(new FakeSettings());
        }

        [Test]
        public void can_completely_replace_the_settings()
        {
            theSettings.Replace(new FakeSettings{Name = "Lindsey", Hometown = "San Antonio"});

            theSettings.Get<FakeSettings>().ShouldEqual(new FakeSettings { Name = "Lindsey", Hometown = "San Antonio" });
        }

        [Test]
        public void can_alter_the_settings_without_replacing_it()
        {
            var original = theSettings.Get<FakeSettings>();

            theSettings.Alter<FakeSettings>(x => {
                Thread.Sleep(500);
                x.Name = "Max";
            });

            theSettings.Get<FakeSettings>().ShouldBeTheSameAs(original);
            theSettings.Get<FakeSettings>().Name.ShouldEqual("Max");
        }

        [Test]
        public void has_explicit_test()
        {
            theSettings.HasExplicit<FakeSettings>().ShouldBeFalse();

            theSettings.Replace(new FakeSettings());

            theSettings.HasExplicit<FakeSettings>().ShouldBeTrue();
        }
    }

    [ApplicationLevel]
    public class AppSettings
    {
        public string Name { get; set; }
    }

    public class FakeSettings
    {
        public string Name = "Jeremy";
        public string Hometown = "Jasper";

        public bool Equals(FakeSettings other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Name, Name) && Equals(other.Hometown, Hometown);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (FakeSettings)) return false;
            return Equals((FakeSettings) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0)*397) ^ (Hometown != null ? Hometown.GetHashCode() : 0);
            }
        }
    }
}