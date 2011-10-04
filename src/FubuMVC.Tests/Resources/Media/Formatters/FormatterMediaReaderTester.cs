using FubuMVC.Core.Resources.Media.Formatters;
using FubuMVC.Core.Runtime;
using FubuMVC.Tests.Resources.Projections;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Resources.Media.Formatters
{
    [TestFixture]
    public class FormatterMediaReaderTester : InteractionContext<FormatterMediaReader<Address>>
    {
        private IFormatter jsonFormatter;
        private IFormatter xmlFormatter;
        private Address theJsonAddress;
        private Address theXmlAddress;

        protected override void beforeEach()
        {
            var formatters = Services.CreateMockArrayFor<IFormatter>(2);
            jsonFormatter = formatters[0];
            xmlFormatter = formatters[1];

            jsonFormatter.Stub(x => x.MatchingMimetypes).Return(new string[]{"text/json", "application/json"});
            xmlFormatter.Stub(x => x.MatchingMimetypes).Return(new string[]{"text/xml", "application/xml"});
        
            theJsonAddress = new Address();
            theXmlAddress = new Address();

            jsonFormatter.Stub(x => x.Read<Address>()).Return(theJsonAddress);
            xmlFormatter.Stub(x => x.Read<Address>()).Return(theXmlAddress);
        }


        [Test]
        public void has_all_the_mime_types_from_inner_formatters()
        {
            ClassUnderTest.Mimetypes.ShouldHaveTheSameElementsAs("text/json", "application/json", "text/xml", "application/xml");
        }

        [Test]
        public void read_from_the_right_formatter_by_mimetype()
        {
            ClassUnderTest.Read("text/json").ShouldBeTheSameAs(theJsonAddress);
            ClassUnderTest.Read("application/json").ShouldBeTheSameAs(theJsonAddress);
            ClassUnderTest.Read("application/xml").ShouldBeTheSameAs(theXmlAddress);
            ClassUnderTest.Read("text/xml").ShouldBeTheSameAs(theXmlAddress);
        }

        [Test]
        public void reading_should_set_properties_from_route_data_as_well()
        {
            ClassUnderTest.Read("text/json");
            MockFor<ISetterBinder>().AssertWasCalled(x => x.BindProperties(theJsonAddress));
        }
    }
}