using FubuCore;
using FubuCore.Formatting;
using FubuHtml.Elements;
using FubuHtml.Elements.Builders;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuHtml.Testing.Elements.Builders
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