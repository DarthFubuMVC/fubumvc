using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing
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

        [Test]
        public void get_absolute_for_unrooted_url_with_empty_stubbed_root()
        {
            UrlContext.Stub("");
            "someUrl".ToAbsoluteUrl().ShouldEqual("someUrl");
        }

        [SetUp]
        public void SetUp()
        {
            UrlContext.Stub("/app");
        }

        [Test]
        public void get_absolute_for_unrooted_url()
        {
            "someUrl".ToAbsoluteUrl().ShouldEqual("/app/someUrl");
        }

        [Test]
        public void get_absolute_for_rooted_url()
        {
            "/folder/someUrl".ToAbsoluteUrl().ShouldEqual("/folder/someUrl");
        }


        [Test]
        public void get_absolute_for_rooted_url_with_route_info()
        {
            "/folder/someUrl/{something}".ToAbsoluteUrl().ShouldEqual("/folder/someUrl/{something}");
        }

        [Test]
        public void get_absolute_for_app_relative_url()
        {
            "~/someUrl".ToAbsoluteUrl().ShouldEqual("/app/someUrl");
        }

        [Test]
        public void get_absolute_for_fully_qualified_url()
        {
            "http://somewhere.com/someUrl".ToAbsoluteUrl().ShouldEqual("http://somewhere.com/someUrl");
        }

        [Test]
        public void get_absolute_for_empty_url()
        {
            "".ToAbsoluteUrl().ShouldEqual("/app/");
        }


        [Test]
        public void get_server_Url_for_unrooted_url()
        {
            "someUrl".ToServerQualifiedUrl(SERVER_BASE).ShouldEqual("http://www.someserver/app/someUrl");
        }

        [Test]
        public void get_server_Url_for_rooted_url()
        {
            "/folder/someUrl".ToServerQualifiedUrl(SERVER_BASE).ShouldEqual("http://www.someserver/folder/someUrl");
        }

        [Test]
        public void get_server_Url_for_app_relative_url()
        {
            "~/someUrl".ToServerQualifiedUrl(SERVER_BASE).ShouldEqual("http://www.someserver/app/someUrl");
        }

        [Test]
        public void get_server_Url_for_fully_qualified_url()
        {
            "http://somewhere.com/someUrl".ToServerQualifiedUrl(SERVER_BASE).ShouldEqual("http://somewhere.com/someUrl");
        }



        [Test]
        public void get_path_for_unrooted_url()
        {
            "someUrl".ToPhysicalPath().ShouldEqual(@"\app\someUrl");
        }

        [Test]
        public void get_path_for_rooted_url()
        {
            "/folder/someUrl".ToPhysicalPath().ShouldEqual(@"\folder\someUrl");
        }

        [Test]
        public void get_path_for_app_relative_url()
        {
            "~/someUrl".ToPhysicalPath().ShouldEqual(@"\app\someUrl");
        }
    }
}