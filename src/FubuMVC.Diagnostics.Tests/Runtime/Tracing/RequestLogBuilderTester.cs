using System;
using FubuCore.Binding;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Urls;
using FubuMVC.Diagnostics.Chains;
using FubuMVC.Diagnostics.Runtime;
using FubuMVC.Diagnostics.Runtime.Tracing;
using FubuTestingSupport;
using NUnit.Framework;
using FubuCore;
using Rhino.Mocks;

namespace FubuMVC.Diagnostics.Tests.Runtime.Tracing
{
    [TestFixture]
    public class RequestLogBuilderTester : InteractionContext<RequestLogBuilder>
    {
        private StubUrlRegistry theUrls;
        private RequestLog theLog;
        private BehaviorChain theOriginatingChain;

        protected override void beforeEach()
        {
            MockFor<IHttpRequest>().Stub(x => x.RelativeUrl())
                .Return("the relative url");

            MockFor<IHttpRequest>().Stub(x => x.HttpMethod())
                .Return("PUT");

            LocalSystemTime = DateTime.Today.Add(5.Hours());

            theOriginatingChain = new RoutedChain("some/url");
            theOriginatingChain.AddToEnd(new OutputNode(GetType()));
            theOriginatingChain.UniqueId.ShouldNotEqual(Guid.Empty);

            MockFor<ICurrentChain>().Stub(x => x.OriginatingChain).Return(theOriginatingChain);


            theUrls = Services.StubUrls();

            theLog = ClassUnderTest.BuildForCurrentRequest();
        }

        [Test]
        public void writes_the_request_data_to_the_log()
        {
            MockFor<IRequestData>().AssertWasCalled(x => x.WriteReport(theLog.RequestData));
        }

        [Test]
        public void sets_the_behavior_id_from_the_originating_chain()
        {
            theLog.ChainId.ShouldEqual(theOriginatingChain.UniqueId);
        }

        [Test]
        public void sets_the_date()
        {
            theLog.Time.ShouldEqual(UtcSystemTime);
        }

        [Test]
        public void sets_the_request_url()
        {
            theLog.Endpoint.ShouldEqual("the relative url");
        }

        [Test]
        public void sets_the_http_method()
        {
            theLog.HttpMethod.ShouldEqual("PUT");
        }
    }
}