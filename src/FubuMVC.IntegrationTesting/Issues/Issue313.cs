using System;
using FubuMVC.Core;
using FubuMVC.TestingHarness;
using HtmlTags;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.IntegrationTesting.Issues
{
    [TestFixture]
    public class Issue313 : FubuRegistryHarness
    {
        protected override void configure(FubuRegistry registry)
        {
            registry.Actions.IncludeType<HomeEndpoint>();
            registry.Routes.HomeIs<HomeEndpoint>(x => x.home(null));
        }

        [Test]
        public void respects_the_query_string_in_the_url_determination()
        {
            endpoints.GetByInput(new HomeInput{
                Name = "Jeremy"
            }).ReadAsText()
                .ShouldEqual("Name is Jeremy");
        }
    }

    [TestFixture]
    public class Issue313_by_model : FubuRegistryHarness
    {
        protected override void configure(FubuRegistry registry)
        {
            registry.Actions.IncludeType<HomeEndpoint>();
            registry.Routes.HomeIs<HomeInput>();
        }

        [Test]
        public void respects_the_query_string_in_the_url_determination()
        {
            endpoints.GetByInput(new HomeInput
            {
                Name = "Jeremy"
            }).ReadAsText()
                .ShouldEqual("Name is Jeremy");
        }
    }

    public class HomeEndpoint
    {
        public string home(HomeInput input)
        {
            return "Name is " + input.Name;
        }
    }

    public class HomeInput
    {
        [QueryString]
        public string Name { get; set; }
    }
}