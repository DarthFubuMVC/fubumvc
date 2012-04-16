using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Formatters;
using FubuMVC.Tests.NewConneg;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Runtime.Formatters
{
    [TestFixture]
    public class JsonFormatterTester : InteractionContext<JsonFormatter>
    {
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