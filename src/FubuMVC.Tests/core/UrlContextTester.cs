using System;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Core.Testing
{
    [TestFixture]
    public class UrlContextTester
    {
        private const string SERVER_BASE = "http://www.someserver/ignored/path";
        /**************************************************************************
         * These tests really only confirm the Stubbed behavior, which is only 
         * useful in the context of other tests.
         * 
         * The real (live) behavior is tested via qunit in the FubuMVC.HelloWorld project.
         * To run the test, start the FubuMVC.HelloWorld project and navigate to:
         *   http://localhost:52010/helloworld/IntegrationTests/run
         *   
         * The source of the tests is in:
         *   FubuMVC.HelloWorld\Controllers\IntegrationTests\RunView.aspx
         **************************************************************************/

        //[Test]
        //public void get_absolute_for_unrooted_url_with_empty_stubbed_root()
        //{
        //    UrlContext.Stub("");
        //    "someUrl".ToAbsoluteUrl().ShouldEqual("someUrl");
        //}

        //[SetUp]
        //public void SetUp()
        //{
        //    UrlContext.Stub("/app");
        //}

        [Test]
        public void get_absolute_for_unrooted_url()
        {
            "someUrl".ToAbsoluteUrl("http://server").ShouldEqual("http://server/someUrl");
        }

        [Test]
        public void get_absolute_for_rooted_url()
        {
            "/folder/someUrl".ToAbsoluteUrl("http://server").ShouldEqual("http://server/folder/someUrl");
        }

        [Test]
        public void get_absolute_for_rooted_url_with_route_info()
        {
            "/folder/someUrl/{something}".ToAbsoluteUrl("http://server").ShouldEqual("http://server/folder/someUrl/{something}");
        }

        [Test]
        public void get_absolute_for_app_relative_url()
        {
            "~/someUrl".ToAbsoluteUrl("http://server/app").ShouldEqual("http://server/app/someUrl");
        }

        [Test]
        public void get_absolute_for_fully_qualified_url()
        {
            "http://somewhere.com/someUrl".ToAbsoluteUrl("http://server").ShouldEqual("http://somewhere.com/someUrl");
        }

        [Test]
        public void get_absolute_for_empty_url()
        {
            "".ToAbsoluteUrl("http://server").ShouldEqual("http://server");
        }

        [Test]
        public void get_server_url_for_fully_qualified_url()
        {
            "http://somewhere.com/someUrl".ToServerQualifiedUrl(SERVER_BASE).ShouldEqual("http://somewhere.com/someUrl");
        }

        [Test]
        public void get_url_without_querystring()
        {
            "/someUrl?query=foo".WithoutQueryString().ShouldEqual(@"/someUrl");
        }

        [Test]
        public void get_url_without_querystring_having_no_querystring_does_nothing()
        {
            "/someUrl".WithoutQueryString().ShouldEqual(@"/someUrl");
        }
    }
}