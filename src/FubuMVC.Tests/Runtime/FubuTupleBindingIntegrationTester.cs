using FubuCore.Binding;
using FubuMVC.Core.Runtime;
using FubuMVC.StructureMap;
using NUnit.Framework;
using InMemoryRequestData = FubuCore.Binding.InMemoryRequestData;
using FubuTestingSupport;

namespace FubuMVC.Tests.Runtime
{
    [TestFixture]
    public class FubuTupleBindingIntegrationTester
    {
        private ETagRequest theEtagRequest;
        private Target theTarget;
        private ETagTuple theTuple;

        [SetUp]
        public void SetUp()
        {
            var request = new InMemoryFubuRequest();
            var container = StructureMapContainerFacility.GetBasicFubuContainer().GetNestedContainer();
            container.Inject<IFubuRequest>(request);

            theEtagRequest = new ETagRequest();
            theTarget = new Target();

            request.Set(theEtagRequest);
            request.Set(theTarget);

            // The FubuTupleBinder should be registered by default
            var binder = container.GetInstance<IObjectResolver>();

            theTuple = binder.BindModel(typeof (ETagTuple), new InMemoryRequestData()).Value.ShouldBeOfType<ETagTuple>();
        }

        [Test]
        public void should_bind_both_properties_with_the_values_from_the_IFubuRequest()
        {
            theTuple.Etag.ShouldBeTheSameAs(theEtagRequest);
            theTuple.Target.ShouldBeTheSameAs(theTarget);
        }

        public class ETagRequest
        {
            public string IfNoneMatch { get; set; }

        }

        public class Target
        {
            
        }

        public class ETagTuple : FubuRequestTuple
        {
            public ETagRequest Etag { get; set; }
            public Target Target { get; set; }
        }
    }
}