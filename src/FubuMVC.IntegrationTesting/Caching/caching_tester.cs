using System;
using System.Net;
using FubuMVC.Core;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Endpoints;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.UI;
using FubuMVC.TestingHarness;
using HtmlTags;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.IntegrationTesting.Caching
{

    [TestFixture]
    public class caching_tester : SharedHarnessContext
    {
        [Test]
        public void simple_vary_by_resource()
        {
            var text1_1 = endpoints.GetByInput(new CachedRequest { Name = "1" }).ReadAsText();
            var text1_2 = endpoints.GetByInput(new CachedRequest { Name = "1" }).ReadAsText();
            var text2_1 = endpoints.GetByInput(new CachedRequest { Name = "2" }).ReadAsText();
            var text2_2 = endpoints.GetByInput(new CachedRequest { Name = "2" }).ReadAsText();


            text1_1.ShouldEqual(text1_2);
            text2_1.ShouldEqual(text2_2);

            text1_1.ShouldNotEqual(text2_1);
        }



    }

    public class CachedEndpoints
    {
        private readonly IOutputWriter _writer;

        public CachedEndpoints(IOutputWriter writer)
        {
            _writer = writer;
        }


        [Cache]
        public HtmlTag get_cached_Name(CachedRequest request)
        {
            return new HtmlTag("h1").Text(Guid.NewGuid().ToString());
        }

        [Cache]
        public HtmlTag DatePartial(DateRequest request)
        {
            return new HtmlTag("h1").Text(Guid.NewGuid().ToString());
        }
    }

    public class CachedRequest
    {
        public string Name { get; set; }
    }

    public class DateRequest{}


}