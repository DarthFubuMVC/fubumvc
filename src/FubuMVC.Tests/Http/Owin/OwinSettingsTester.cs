using System.Linq;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Http.Owin.Middleware;
using FubuMVC.Core.Http.Owin.Middleware.StaticFiles;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Http.Owin
{
    [TestFixture]
    public class OwinSettingsTester
    {
        [Test]
        public void static_file_middle_ware_is_added_by_default()
        {
            var settings = new OwinSettings();

            settings.Middleware.OfType<MiddlewareNode<StaticFileMiddleware>>()
                .Count().ShouldEqual(1);
        }

        
    }
}