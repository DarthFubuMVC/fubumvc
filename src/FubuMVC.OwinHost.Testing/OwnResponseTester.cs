//using System.Net;
//using NUnit.Framework;
//using FubuTestingSupport;

//namespace FubuMVC.OwinHost.Testing
//{
//    [TestFixture]
//    public class OwnResponseTester
//    {
//        private Response response;

//        [SetUp]
//        protected void beforeEach()
//        {
//            response = new Response(null);
//        }

//        [Test]
//        public void should_render_response_status_only()
//        {
//            response.SetStatus(HttpStatusCode.InternalServerError);
//            response.Status.ShouldEqual("500");
//        }

//        [Test]
//        public void should_render_response_status_and_description()
//        {
//            response.SetStatus(HttpStatusCode.NotAcceptable, "your mom goes to college");
//            response.Status.ShouldEqual("406 your mom goes to college");
//        }
//    }
//}