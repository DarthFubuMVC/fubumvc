using FubuMVC.Spark.Rendering;
using FubuTestingSupport;
using NUnit.Framework;
using Spark;

namespace FubuMVC.Spark.Tests.Rendering
{
    [TestFixture]
    public class SparkDescriptorResolverTester : InteractionContext<SparkDescriptorResolver>
    {
        private ViewDefinition _viewDefinition;
        private SparkViewDescriptor _resolvedViewDescriptor;
        private SparkViewDescriptor _resolvedPartialViewDescriptor;

        protected override void beforeEach()
        {
            var viewDescriptor = new SparkViewDescriptor();
            var partialViewDescriptor = new SparkViewDescriptor();
            _viewDefinition = new ViewDefinition(viewDescriptor, partialViewDescriptor, null);

            _resolvedViewDescriptor = ClassUnderTest.ResolveDescriptor(_viewDefinition);
            _resolvedPartialViewDescriptor = ClassUnderTest.ResolvePartialDescriptor(_viewDefinition);
        }

        [Test]
        public void the_resolved_view_descriptor_is_equal_to_the_view_definition_view_descriptor()
        {
            _resolvedViewDescriptor.ShouldEqual(_viewDefinition.ViewDescriptor);
        }

        [Test]
        public void the_resolved_partial_view_descriptor_is_equal_to_the_view_definition_partial_view_descriptor()
        {
            _resolvedPartialViewDescriptor.ShouldEqual(_viewDefinition.PartialDescriptor);
        }
    }
}