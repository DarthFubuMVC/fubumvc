using FubuCore.Binding;
using FubuCore.Binding.InMemory;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Registration
{
    [TestFixture]
    public class ModelBindingServiceRegistry_specification
    {
        private void registeredTypeIs<TService, TImplementation>()
        {
            FubuMode.Reset();
            BehaviorGraph.BuildEmptyGraph().Services.DefaultServiceFor<TService>().Type.ShouldEqual(
                typeof(TImplementation));
        }

        [Test]
        public void BindingContext_is_registered()
        {
            registeredTypeIs<IBindingContext, BindingContext>();
        }

        [Test]
        public void IObjectResolver_is_registered()
        {
            registeredTypeIs<IObjectResolver, ObjectResolver>();
        }

        [Test]
        public void default_binding_logger_is_nullo()
        {
            registeredTypeIs<IBindingLogger, NulloBindingLogger>();
        }

        [Test]
        public void smart_request_is_registered_as_the_fubu_smart_request()
        {
            registeredTypeIs<ISmartRequest, FubuSmartRequest>();
        }
    }
}