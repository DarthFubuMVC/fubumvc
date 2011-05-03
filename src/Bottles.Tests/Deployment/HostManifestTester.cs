using Bottles.Deployment;
using FubuCore.Configuration;
using NUnit.Framework;
using FubuTestingSupport;

namespace Bottles.Tests.Deployment
{
    [TestFixture]
    public class HostManifestTester
    {
        private SettingsData data1;
        private SettingsData data2;
        private SettingsData data3;
        private SettingsData data4;
        private SettingsData data5;

        [SetUp]
        public void SetUp()
        {
            data1 = new SettingsData(SettingCategory.core)
                .With("OneDirective.Name", "Max");


            data2 = new SettingsData(SettingCategory.core)
                .With("ThreeDirective.Threshold", "3");

            data3 = new SettingsData(SettingCategory.core)
                .With("TwoDirective.City", "Austin")
                .With("TwoDirective.IsDomestic", "true");

            data4 = new SettingsData(SettingCategory.core)
                .With("OneDirective.Age", "7");

            data5 = new SettingsData(SettingCategory.core)
                .With("OneDirective.Age", "8");

        }

        [Test]
        public void get_all_unique_directive_names()
        {
            var host = new HostManifest("h1");
            
            host.RegisterSettings(data1);
            host.UniqueDirectiveNames().ShouldHaveTheSameElementsAs("OneDirective");

            host.RegisterSettings(data2);
            host.UniqueDirectiveNames().ShouldHaveTheSameElementsAs("OneDirective", "ThreeDirective");

            host.RegisterSettings(data3);
            host.UniqueDirectiveNames().ShouldHaveTheSameElementsAs("OneDirective", "ThreeDirective", "TwoDirective");

            host.RegisterSettings(data4);
            host.UniqueDirectiveNames().ShouldHaveTheSameElementsAs("OneDirective", "ThreeDirective", "TwoDirective");

            host.RegisterSettings(data5);
            host.UniqueDirectiveNames().ShouldHaveTheSameElementsAs("OneDirective", "ThreeDirective", "TwoDirective");
        }

        [Test]
        public void append_host_respects_setting_order()
        {
            var host1 = new HostManifest("h1");
            host1.RegisterSettings(data4);


            var host2 = new HostManifest("h2");
            host2.RegisterSettings(data5);


            host1.Append(host2);

            host1.GetDirective<OneDirective>().Age.ShouldEqual(7);
        }

        
        [Test]
        public void append_host_imports_bottle_references()
        {
            var host1 = new HostManifest("h1");
            host1.RegisterBottle(new BottleReference("b1"));


            var host2 = new HostManifest("h2");
            host2.RegisterBottle(new BottleReference("b2"));


            host1.Append(host2);

            host1.BottleReferences.ShouldHaveTheSameElementsAs(new BottleReference("b1"), new BottleReference("b2"));
        }

        [Test]
        public void append_host_imports_bottle_references_but_does_not_duplicate()
        {
            var host1 = new HostManifest("h1");
            host1.RegisterBottle(new BottleReference("b1"));


            var host2 = new HostManifest("h2");
            host2.RegisterBottle(new BottleReference("b1"));


            host1.Append(host2);

            host1.BottleReferences.ShouldHaveTheSameElementsAs(new BottleReference("b1"));
        }


        [Test]
        public void does_not_set_a_property_that_is_not_explicitly_configured()
        {
            var host = new HostManifest("h1");
            host.RegisterSettings(data4);

            // the default value for Name is "somebody"
            host.GetDirective<OneDirective>().Name.ShouldEqual("somebody");
        }

        [Test]
        public void respects_ordering_of_setting_data()
        {
            var host1 = new HostManifest("h1");
            host1.RegisterSettings(data4);
            host1.RegisterSettings(data5);

            host1.GetDirective<OneDirective>().Age.ShouldEqual(7);

            var host2 = new HostManifest("h2");
            host2.RegisterSettings(data5);
            host2.RegisterSettings(data4);

            host2.GetDirective<OneDirective>().Age.ShouldEqual(8);
        }

        [Test]
        public void can_pull_setting_class_out_of_request_data()
        {
            var host = new HostManifest("host1");
            host.RegisterSettings(data3);

            var directive = host.GetDirective<TwoDirective>();
            directive.City.ShouldEqual("Austin");
            directive.IsDomestic.ShouldBeTrue();
        }

        [Test]
        public void can_pull_setting_class_out_of_multiple_data_settings_with_no_conflict()
        {
            var host = new HostManifest("host1");
            host.RegisterSettings(data1);
            host.RegisterSettings(data4);

            var directive = host.GetDirective<OneDirective>();
            directive.Name.ShouldEqual("Max");
            directive.Age.ShouldEqual(7);
        
        }

        [Test]
        public void can_pull_complex_settings()
        {
            var host = new HostManifest("host1");

            var data = new SettingsData()
                .With("ComplexDirective.One.Name", "Thurgood")
                .With("ComplexDirective.One.Age", "57")
                .With("ComplexDirective.Two.City", "Joplin");

            host.RegisterSettings(data);

            var complex = host.GetDirective<ComplexDirective>();

            complex.One.Name.ShouldEqual("Thurgood");
            complex.One.Age.ShouldEqual(57);
            complex.Two.City.ShouldEqual("Joplin");
        }

        [Test]
        public void do_not_bind_complex_child_if_no_data_exists()
        {
            var host = new HostManifest("host1");

            var data = new SettingsData()
                .With("ComplexDirective.One.Name", "Thurgood")
                .With("ComplexDirective.One.Age", "57");

            host.RegisterSettings(data);

            var complex = host.GetDirective<ComplexDirective>();
            complex.One.ShouldNotBeNull();
            complex.Two.ShouldBeNull();
        
        }
        
    }

    public class ComplexDirective : IDirective
    {
        public OneDirective One { get; set; }
        public TwoDirective Two { get; set; }
    }

    public class OneDirective : IDirective
    {
        public OneDirective()
        {
            Name = "somebody";
        }

        public string Name { get; set; }
        public int Age { get; set; }
    }
    
    public class TwoDirective : IDirective
    {
        public string City { get; set; }
        public bool IsDomestic { get; set; }
    }

    public class ThreeDirective : IDirective
    {
        public int Threshold { get; set; }
        public string Direction { get; set; }
    }
}