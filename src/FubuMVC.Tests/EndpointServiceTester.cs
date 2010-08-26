using System;
using FubuMVC.Core;
using FubuMVC.Core.Security;
using FubuMVC.Core.Urls;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class EndpointServiceTester : InteractionContext<EndpointService>
    {
        private IUrlRegistry urls;
        private IAuthorizationPreviewService authorizor;

        protected override void beforeEach()
        {
            urls = MockFor<IUrlRegistry>();
            authorizor = MockFor<IAuthorizationPreviewService>();
        }

        [Test]
        public void by_model()
        {
            var model = new TestOutputModel();

            urls.Stub(x => x.UrlFor(model)).Return("url1");
            authorizor.Stub(x => x.IsAuthorized(model)).Return(true);

            ClassUnderTest.EndpointFor(model).ShouldEqual(new Endpoint{
                IsAuthorized = true,
                Url = "url1"
            });
        }

        [Test]
        public void by_model_and_category()
        {
            var model = new TestOutputModel();

            var theCategory = "Category A";
            urls.Stub(x => x.UrlFor(model, theCategory)).Return("url1");
            authorizor.Stub(x => x.IsAuthorized(model, theCategory)).Return(true);

            ClassUnderTest.EndpointFor(model, theCategory).ShouldEqual(new Endpoint
            {
                IsAuthorized = true,
                Url = "url1"
            });
        }

        [Test]
        public void by_handler_type_and_method()
        {
            var handlerType = this.GetType();
            var method = handlerType.GetMethods().First();

            urls.Stub(x => x.UrlFor(handlerType, method)).Return("url1");
            authorizor.Stub(x => x.IsAuthorized(handlerType, method)).Return(true);

            ClassUnderTest.EndpointFor(handlerType, method).ShouldEqual(new Endpoint
            {
                IsAuthorized = true,
                Url = "url1"
            });
        }

        [Test]
        public void by_new_of_an_entity()
        {
            urls.Stub(x => x.UrlForNew(GetType())).Return("url1");
            authorizor.Stub(x => x.IsAuthorizedForNew(GetType())).Return(true);

            ClassUnderTest.EndpointForNew(GetType()).ShouldEqual(new Endpoint
            {
                IsAuthorized = true,
                Url = "url1"
            });
        }

        [Test]
        public void has_new_endpoint()
        {
            urls.Stub(x => x.HasNewUrl(GetType())).Return(true);

            ClassUnderTest.HasNewEndpoint(GetType()).ShouldBeTrue();
        }

        [Test]
        public void by_property_update_model()
        {
            var model = new TestOutputModel();

            urls.Stub(x => x.UrlForPropertyUpdate(model)).Return("url1");
            authorizor.Stub(x => x.IsAuthorizedForPropertyUpdate(model)).Return(true);

            ClassUnderTest.EndpointForPropertyUpdate(model).ShouldEqual(new Endpoint
            {
                IsAuthorized = true,
                Url = "url1"
            });
        }


        [Test]
        public void by_property_update_type()
        {

            urls.Stub(x => x.UrlForPropertyUpdate(GetType())).Return("url1");
            authorizor.Stub(x => x.IsAuthorizedForPropertyUpdate(GetType())).Return(true);

            ClassUnderTest.EndpointForPropertyUpdate(GetType()).ShouldEqual(new Endpoint
            {
                IsAuthorized = true,
                Url = "url1"
            });
        }
    }
}