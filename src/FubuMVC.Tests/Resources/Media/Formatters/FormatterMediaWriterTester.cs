using FubuMVC.Core.Http;
using FubuMVC.Core.Resources.Media.Formatters;
using FubuMVC.Core.Runtime;
using FubuMVC.Tests.Resources.Projections;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Resources.Media.Formatters
{
    [TestFixture]
    public class FormatterMediaWriterTester : InteractionContext<FormatterMediaWriter<Address>>
    {
        private IFormatter jsonFormatter;
        private IFormatter xmlFormatter;
        private CurrentMimeType theCurrentMimeTypes;
        private Address theAddress;

        protected override void beforeEach()
        {
            var formatters = Services.CreateMockArrayFor<IFormatter>(2);
            jsonFormatter = formatters[0];
            xmlFormatter = formatters[1];

            jsonFormatter.Stub(x => x.MatchingMimetypes).Return(new string[] { "text/json", "application/json" });
            xmlFormatter.Stub(x => x.MatchingMimetypes).Return(new string[] { "text/xml", "application/xml" });

            theCurrentMimeTypes = new CurrentMimeType("", "");
            MockFor<IFubuRequest>().Stub(x => x.Get<CurrentMimeType>())
                .Return(theCurrentMimeTypes);

            theAddress = new Address();
        }

        public void ShouldHaveWrittenJson(string mimeType)
        {
            jsonFormatter.AssertWasCalled(x => x.Write(theAddress, mimeType));
        }

        public void ShouldHaveWrittenXml(string mimeType)
        {
            xmlFormatter.AssertWasCalled(x => x.Write(theAddress, mimeType));
        }

        [Test]
        public void has_all_the_mime_types_from_all_the_formatters()
        {
            ClassUnderTest.Mimetypes
                .ShouldHaveTheSameElementsAs("text/json", "application/json", "text/xml", "application/xml");
        }

        [Test]
        public void select_the_correct_formatter_by_mime_type_1()
        {
            theCurrentMimeTypes.AcceptTypes.AddMimeType("text/xml");
            ClassUnderTest.Write(theAddress);
        
            ShouldHaveWrittenXml("text/xml");
        }


        [Test]
        public void select_the_correct_formatter_by_mime_type_2()
        {
            theCurrentMimeTypes.AcceptTypes.AddMimeType("text/xml");
            theCurrentMimeTypes.AcceptTypes.AddMimeType("text/json");
            ClassUnderTest.Write(theAddress);

            ShouldHaveWrittenXml("text/xml");
        }

        [Test]
        public void select_the_correct_formatter_by_mime_type_3()
        {
            theCurrentMimeTypes.AcceptTypes.AddMimeType("text/json,text/xml");
            ClassUnderTest.Write(theAddress);

            ShouldHaveWrittenJson("text/json");
        }

        [Test]
        public void select_the_first_formatter_if_none_match()
        {
            ClassUnderTest.Write(theAddress);
            ShouldHaveWrittenJson("text/json");
        }
    }
}