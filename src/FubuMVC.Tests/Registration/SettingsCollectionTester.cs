using System.Threading;
using FubuMVC.Core.Registration;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Registration
{


    [TestFixture]
    public class SettingsCollection_without_a_parent_Tester
    {
        private SettingsCollection theSettings;

        [SetUp]
        public void SetUp()
        {
            theSettings = new SettingsCollection();
        }

        [Test]
        public void get_can_happily_create_the_default()
        {
            theSettings.Get<FakeSettings>().ShouldBe(new FakeSettings());
        }

        [Test]
        public void can_completely_replace_the_settings()
        {
            theSettings.Replace(new FakeSettings {Name = "Lindsey", Hometown = "San Antonio"});

            theSettings.Get<FakeSettings>().ShouldBe(new FakeSettings {Name = "Lindsey", Hometown = "San Antonio"});
        }

        [Test]
        public void can_alter_the_settings_without_replacing_it()
        {
            var original = theSettings.Get<FakeSettings>();

            theSettings.Alter<FakeSettings>(x =>
            {
                Thread.Sleep(500);
                x.Name = "Max";
            });

            theSettings.Get<FakeSettings>().ShouldBeTheSameAs(original);
            theSettings.Get<FakeSettings>().Name.ShouldBe("Max");
        }

        [Test]
        public void has_explicit_test()
        {
            theSettings.HasExplicit<FakeSettings>().ShouldBeFalse();

            theSettings.Replace(new FakeSettings());

            theSettings.HasExplicit<FakeSettings>().ShouldBeTrue();
        }
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