using System.Collections.Generic;
using FubuCore.Configuration;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuCore.Testing.Configuration
{
    [TestFixture]
    public class XmlSettingsIntegratedTester
    {
        private IEnumerable<SettingsData> theData;

        [SetUp]
        public void SetUp()
        {
            // See the *.config files in this directory in solution
            // explorer
            var source = new FolderAppSettingsXmlSource("Configuration");
            theData = source.FindSettingData();
        }

        [Test]
        public void found_all_four_sources_of_data()
        {
            theData.Select(x => x.Provenance).OrderBy(x => x)
                .ShouldHaveTheSameElementsAs(
                    "Configuration\\Environment.config", 
                    "Configuration\\One.config", 
                    "Configuration\\Three.config",
                    "Configuration\\Two.config");    
        }

        [Test]
        public void smoke_test_can_read_data()
        {
            var data = theData.First(x => x.Provenance.Contains("Environment"));
            data.Get("OneSettings.Name").ShouldEqual("Max");
        }
    }

    public class OneSettings
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class TwoSettings
    {
        public string City { get; set; }
        public bool IsLocal { get; set; }
    }

    public class ThreeSettings
    {
        public int Threshold { get; set; }
        public string Direction { get; set; }
    }
}