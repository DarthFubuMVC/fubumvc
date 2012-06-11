using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.WebForms.Testing
{
    [TestFixture]
    public class WebForms_service_registration_tester
    {
        private void registeredTypeIs<TService, TImplementation>()
        {
            var fubuRegistry = new FubuRegistry();
            fubuRegistry.Import<WebFormsEngine>();

            BehaviorGraph.BuildFrom(fubuRegistry).Services.DefaultServiceFor<TService>().Type.ShouldEqual(
                typeof(TImplementation));
        }


        [Test]
        public void IWebFormsControlBuilder_is_registered()
        {
            registeredTypeIs<IWebFormsControlBuilder, WebFormsControlBuilder>();
        }

        [Test]
        public void IWebRenderer_is_registered()
        {
            registeredTypeIs<IWebFormRenderer, WebFormRenderer>();
        }
    }
}