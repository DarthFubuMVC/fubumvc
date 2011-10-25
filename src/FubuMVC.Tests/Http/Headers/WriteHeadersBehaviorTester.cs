using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Headers;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Http.Headers
{
    [TestFixture]
    public class WriteHeadersBehaviorTester : InteractionContext<WriteHeadersBehavior>
    {
        protected override void beforeEach()
        {
            var headers1 = new HttpHeaderValues();
            headers1["a"] = "1";
            headers1["b"] = "2";

            var headers2 = new HttpHeaderValues();
            headers2["c"] = "3";
            headers2["d"] = "4";

            MockFor<IFubuRequest>().Stub(x => x.Find<IHaveHeaders>()).Return(new IHaveHeaders[]{headers1, headers2});

            ClassUnderTest.Invoke();
        }

        [Test]
        public void should_write_all_possible_headers()
        {
            MockFor<IHttpWriter>().AssertWasCalled(x => x.AppendHeader("a", "1"));
            MockFor<IHttpWriter>().AssertWasCalled(x => x.AppendHeader("b", "2"));
            MockFor<IHttpWriter>().AssertWasCalled(x => x.AppendHeader("c", "3"));
            MockFor<IHttpWriter>().AssertWasCalled(x => x.AppendHeader("d", "4"));
        }
    }
}