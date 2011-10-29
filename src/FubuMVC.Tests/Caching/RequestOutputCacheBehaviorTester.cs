using System;
using System.IO;
using System.Net;
using System.Web;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Caching
{
    [TestFixture]
    public class RequestOutputCacheBehaviorTester : InteractionContext<RequestOutputCacheBehavior<TestObj>>
    {
        private TestObj theModel;
        private CacheOptions<TestObj> theOptions;

        protected override void beforeEach()
        {
            theOptions = new CacheOptions<TestObj>(obj=>obj.Name);
            Services.Inject<IRequestOutputCache<TestObj>>(new RequestOutputCache<TestObj>(MockFor<ICacheProvider>(), theOptions));
            Services.Inject<IOutputWriter>(new StubOutputWriter());

            theModel = new TestObj() {Name = "fubu"};
            MockFor<IFubuRequest>().Stub(req => req.Get<TestObj>()).Return(theModel);
            MockFor<ICacheProvider>().Stub(cp => cp.Get("fubu")).Return(null);
        }

        [Test]
        public void Invoke()
        {
            ClassUnderTest.Invoke();
            
            MockFor<IActionBehavior>().AssertWasCalled(a=>a.Invoke());
        }

        [Test]
        public void InvokePartial()
        {
            ClassUnderTest.InvokePartial();
            
            MockFor<IActionBehavior>().AssertWasCalled(a => a.InvokePartial());
        }

        class StubOutputWriter : IOutputWriter
        {
            public void WriteFile(string contentType, string localFilePath, string displayName)
            {
                throw new NotImplementedException();
            }

            public void Write(string contentType, string renderedOutput)
            {
                //do i need to test?
            }

            public void RedirectToUrl(string url)
            {
                throw new NotImplementedException();
            }

            public void AppendCookie(HttpCookie cookie)
            {
                throw new NotImplementedException();
            }

            public void AppendHeader(string key, string value)
            {
                throw new NotImplementedException();
            }

            public void Write(string contentType, Action<Stream> output)
            {
                throw new NotImplementedException();
            }

            public void WriteResponseCode(HttpStatusCode status)
            {
                throw new NotImplementedException();
            }

            public OldRecordedOutput Record(Action action)
            {
                action();
                return new OldRecordedOutput("","");
            }
        }

    }
}