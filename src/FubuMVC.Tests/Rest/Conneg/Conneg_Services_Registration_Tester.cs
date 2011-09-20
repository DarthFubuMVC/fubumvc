using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Rest.Conneg;
using FubuMVC.Core.Rest.Media.Formatters;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuMVC.Tests.Rest.Conneg
{
    [TestFixture]
    public class Conneg_Services_Registration_Tester
    {
        private IServiceRegistry theServices;

        [SetUp]
        public void SetUp()
        {
            theServices = new FubuRegistry().BuildGraph().Services;
        }

        [Test]
        public void JsonFormatter_is_registered()
        {
            theServices.ServicesFor<IFormatter>().Single(x => x.Type == typeof (JsonFormatter))
                .ShouldNotBeNull();
        }

        [Test]
        public void XmlFormatter_is_registered()
        {
            theServices.ServicesFor<IFormatter>().Single(x => x.Type == typeof(XmlFormatter))
                .ShouldNotBeNull();
        }

    }
}