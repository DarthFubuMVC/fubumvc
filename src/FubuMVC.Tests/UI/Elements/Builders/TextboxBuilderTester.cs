using FubuCore;
using FubuCore.Formatting;
using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.UI.Elements.Builders;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.UI.Elements.Builders
{
    [TestFixture]
    public class TextboxBuilderTester
    {
        private ElementRequest theRequest;
        private Address theAddress;

        [SetUp]
        public void SetUp()
        {
            theRequest = ElementRequest.For<Address>(x => x.Address1);
            theAddress = new Address{
                Address1 = "22 Cherry Tree Lane"
            };

            var services = new InMemoryServiceLocator();
            services.Add(new Stringifier());
            theRequest.Attach(services);

            theRequest.Model = theAddress;
        }

        [Test]
        public void build_a_text_box_with__the_value()
        {
            new TextboxBuilder().Build(theRequest).ToString()
                .ShouldEqual("<input type=\"text\" value=\"22 Cherry Tree Lane\" />");
        }
    }
}