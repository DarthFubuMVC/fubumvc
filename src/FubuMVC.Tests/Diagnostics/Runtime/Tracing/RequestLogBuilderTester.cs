using System;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics.Runtime;
using FubuMVC.Core.Diagnostics.Runtime.Tracing;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Urls;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Diagnostics.Runtime.Tracing
{
    [TestFixture, Ignore("come back to this. Functionality changed underneath it")]
    public class RequestLogBuilderTester : InteractionContext<RequestLogBuilder>
    {
        private StubUrlRegistry theUrls;
        private RequestLog theLog;
        private RoutedChain theOriginatingChain;

        protected override void beforeEach()
        {
            FubuMode.Reset();

            MockFor<IHttpRequest>().Stub(x => x.RelativeUrl())
                .Return("the relative url");

            MockFor<IHttpRequest>().Stub(x => x.HttpMethod())
                .Return("PUT");

            LocalSystemTime = DateTime.Today.Add(5.Hours());

            theOriginatingChain = new RoutedChain("some/url");
            theOriginatingChain.AddToEnd(new OutputNode(GetType()));
            theOriginatingChain.UniqueId.ShouldNotEqual(Guid.Empty);

            MockFor<ICurrentChain>().Stub(x => x.OriginatingChain).Return(theOriginatingChain);
            MockFor<ICurrentChain>().Stub(x => x.IsInPartial()).Return(false);


            theUrls = Services.StubUrls();

            theLog = ClassUnderTest.BuildForCurrentRequest();
        }


        [Test]
        public void sets_the_behavior_id_from_the_originating_chain()
        {
            theLog.Hash.ShouldEqual(theOriginatingChain.Title().GetHashCode());
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