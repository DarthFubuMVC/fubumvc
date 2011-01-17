using FubuCore.Binding;
using NUnit.Framework;
using InMemoryRequestData=FubuMVC.Core.Runtime.InMemoryRequestData;

namespace FubuMVC.Tests.Runtime
{
    [TestFixture]
    public class PrefixedRequestDataTester
    {
        private InMemoryRequestData inner;
        private PrefixedRequestData prefixed;

        [SetUp]
        public void SetUp()
        {
            inner = new InMemoryRequestData();
            prefixed = new PrefixedRequestData(inner, "Site");
        }

        [Test]
        public void read_the_inner_by_adding_the_prefix()
        {
            inner["SiteName"] = "a";
            inner["Name"] = "b";

            prefixed.Value("Name").ShouldEqual("a");
        }

        [Test]
        public void read_the_inner_by_adding_the_prefix_in_the_continuation_mode()
        {
            string name = null;

            inner["SiteName"] = "Jeremy";

            prefixed.Value("Name", o => name = (string) o);

            name.ShouldEqual("Jeremy");
        }

        [Test]
        public void miss_on_the_continuation()
        {
            inner["Name"] = "Jeremy";
            prefixed.Value("Name", o => Assert.Fail("Should not be calling back in the continuation"));
        }

        [Test]
        public void positive_on_any_values_prefixed()
        {
            inner["SiteAddress[0]Name"] = "Jeremy";
        
            prefixed.HasAnyValuePrefixedWith("Address[0]").ShouldBeTrue();
            prefixed.HasAnyValuePrefixedWith("Address[1]").ShouldBeFalse();
        }
    }
}