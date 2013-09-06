using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime.Formatters;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Resources.Conneg
{
    [TestFixture]
    public class Conneg_Services_Registration_Tester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            theServices = BehaviorGraph.BuildEmptyGraph().Services;
        }

        #endregion

        private ServiceGraph theServices;

        [Test]
        public void JsonFormatter_is_registered()
        {
            theServices.ServicesFor<IFormatter>().Single(x => x.Type == typeof (JsonFormatter))
                .ShouldNotBeNull();
        }

        [Test]
        public void XmlFormatter_is_registered()
        {
            theServices.ServicesFor<IFormatter>().Single(x => x.Type == typeof (XmlFormatter))
                .ShouldNotBeNull();
        }

        [Test]
        public void default_resource_not_found_handler_is_registered()
        {
            theServices.DefaultServiceFor<IResourceNotFoundHandler>()
                .Type.ShouldEqual(typeof (DefaultResourceNotFoundHandler));
        }
    }
}