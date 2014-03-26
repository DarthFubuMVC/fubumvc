using System;
using FubuMVC.Core;
using FubuMVC.Core.UI;
using FubuMVC.Core.View;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.UI
{
    [TestFixture]
    public class AuthorizedLinkExpressionsTester
    {
        private IFubuPage page;
        private IEndpointService endpoints;
        private Endpoint theEndpoint;


        [SetUp]
        public void SetUp()
        {
            _resultingTag = null;

            page = MockRepository.GenerateMock<IFubuPage>();
            endpoints = MockRepository.GenerateMock<IEndpointService>();

            page.Expect(x => x.Get<IEndpointService>()).Return(endpoints);

            theFinder = s => s.EndpointForNew<SomeEntity>();

            theEndpoint = new Endpoint(){
                IsAuthorized = false,
                Url = "some url"
            };

            endpoints.Expect(x => x.EndpointForNew<SomeEntity>()).Return(theEndpoint);
        }

        private HtmlTag _resultingTag;
        private Func<IEndpointService, Endpoint> theFinder;

        private HtmlTag theResultingTag
        {
            get
            {
                if (_resultingTag == null)
                {
                    _resultingTag = page.AuthorizedLinkTo(theFinder);
                }

                return _resultingTag;
            }
        }

        [Test]
        public void get_a_link_for_an_authorized_endpoint()
        {
            theEndpoint.IsAuthorized = true;

            theResultingTag.Attr("href").ShouldEqual(theEndpoint.Url);
            theResultingTag.Authorized().ShouldBeTrue();

            theResultingTag.ToString().ShouldNotBeEmpty();
        }

        [Test]
        public void get_a_link_for_an_endpoint_that_is_not_authorized()
        {
            theEndpoint.IsAuthorized = false;

            theResultingTag.Authorized().ShouldBeFalse();

            theResultingTag.ToString().ShouldBeEmpty();
        }

    }

    public class SomeEntity{}
}