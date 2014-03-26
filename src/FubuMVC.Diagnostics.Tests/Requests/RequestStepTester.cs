using FubuMVC.Diagnostics.Requests;
using FubuMVC.Diagnostics.Runtime;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Diagnostics.Tests.Requests
{
    [TestFixture]
    public class RequestStepTester
    {
        [Test]
        public void write_simple_content()
        {
            var step = new RequestStep(15, new object());

            var tag = new RequestStepTag(step, "some content");

            tag.Id().ShouldEqual(step.Id.ToString());
        }
    }
}