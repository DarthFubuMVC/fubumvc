using System;
using System.Net;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Headers;
using FubuMVC.Core.Resources.Etags;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Resources.Etags
{
    [TestFixture]
    public class ETagHandlerTester : InteractionContext<ETagHandler<ETagHandlerTester.ResourceOfSomeKind>>
    {
        private string theCurrentEtag = "12345";
        private string theNewEtag = "123456";
        private ETaggedRequest theETaggedRequest;
        private ETagTuple<ResourceOfSomeKind> theEtagTuple;

        protected override void beforeEach()
        {
            theETaggedRequest = new ETaggedRequest(){
                IfNoneMatch = Guid.NewGuid().ToString(),
                ResourcePath = "/something/here"
            };

            theEtagTuple = new ETagTuple<ResourceOfSomeKind>{
                Request = theETaggedRequest,
                Target = new ResourceOfSomeKind()
            };

            MockFor<IEtagCache>().Stub(x => x.CurrentETag(theETaggedRequest.ResourcePath))
                .Return(theCurrentEtag);

            MockFor<IETagGenerator<ResourceOfSomeKind>>().Stub(x => x.Create(null))
                .Return(theNewEtag)
                .IgnoreArguments();
        }

        [Test]
        public void when_matching_a_request_that_has_the_current_etag_should_stop_with_a_NotModified_status_code()
        {
            theETaggedRequest.IfNoneMatch = theCurrentEtag;
            ClassUnderTest.Matches(theETaggedRequest).AssertWasEndedWithStatusCode(HttpStatusCode.NotModified);
        }

        [Test]
        public void when_matching_a_request_that_has_a_different_etag_than_the_current_just_continuje()
        {
            theETaggedRequest.IfNoneMatch = "something different than the current etag";
            ClassUnderTest.Matches(theETaggedRequest).AssertWasContinuedToNextBehavior();
        }

        [Test]
        public void create_an_etag_writes_the_new_etag()
        {
            ClassUnderTest.CreateETag(theEtagTuple);

            MockFor<IEtagCache>().AssertWasCalled(x => x.WriteCurrentETag(theETaggedRequest.ResourcePath, theNewEtag));
        }

        [Test]
        public void registers_an_etag_header()
        {
            ClassUnderTest.CreateETag(theEtagTuple).Headers.ShouldHaveTheSameElementsAs(new Header(HttpResponseHeaders.ETag, theNewEtag));
        }


        public class ResourceOfSomeKind{}
    }
}