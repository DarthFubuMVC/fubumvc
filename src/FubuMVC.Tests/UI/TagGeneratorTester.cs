using FubuCore;
using FubuMVC.Core.UI.Configuration;
using FubuMVC.Core.UI.Tags;
using NUnit.Framework;

namespace FubuMVC.Tests.UI
{
    [TestFixture]
    public class TagGeneratorTester
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void use_the_prefix_in_determining_element_names()
        {
            var naming = new DefaultElementNamingConvention();
            var generator = new TagGenerator<AddressViewModel>(new TagProfileLibrary(), naming, null)
            {
                Model = new AddressViewModel(),
                ElementPrefix = "Site"
            };

            generator.GetRequest(x => x.Address.Address1).ElementId.ShouldEqual("SiteAddressAddress1");
        }
    }
}