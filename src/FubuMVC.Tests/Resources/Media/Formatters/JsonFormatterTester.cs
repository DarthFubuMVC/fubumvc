using System.IO;
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
    public class JsonFormatterTester : InteractionContext<JsonFormatter>
    {
        protected override void beforeEach()
        {
            MockFor<IStreamingData>().Stub(x => x.Output).Return(new MemoryStream());
        }

        [Test]
        public void has_the_right_mime_types()
        {
            ClassUnderTest.MatchingMimetypes.ShouldHaveTheSameElementsAs("application/json", "text/json");
        }

        [Test]
        public void writes_with_the_correct_mimetype_passed_into_it()
        {
            var theAddress = new Address();
            ClassUnderTest.Write(theAddress, "application/json");
            MockFor<IJsonWriter>().AssertWasCalled(x => x.Write(theAddress, "application/json"));
        }
    }


}