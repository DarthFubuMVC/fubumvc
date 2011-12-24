using FubuMVC.Spark.Registration;
using FubuMVC.Spark.Rendering;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests.Rendering
{
    [TestFixture]
    public class DefaultViewDefinitionPolicyTester : InteractionContext<DefaultViewDefinitionPolicy>
    {
        private ViewDescriptor _descriptor;
        private ViewDefinition _definition;
        protected override void beforeEach()
        {
            var template = new Template("view.spark", "root", "pak1");
            var master = new Template("master.spark", "root", "pak1");
            template.Descriptor = new ViewDescriptor(template)
            {
                Master = master
            };
            _descriptor = new ViewDescriptor(template);
            _definition = ClassUnderTest.Create(_descriptor);
        }

        [Test]
        public void matches_is_always_true()
        {
            ClassUnderTest.Matches(null).ShouldBeTrue();
        }

        [Test]
        public void view_definition_is_not_null()
        {
            _definition.ShouldNotBeNull();
        }

        [Test]
        public void spark_view_descriptor_is_built_using_the_template_descriptor()
        {
            _definition.ViewDescriptor.ShouldEqual(_descriptor.ToSparkViewDescriptor());
        }

        [Test]
        public void spark_partial_view_descriptor_is_built_using_the_template_descriptor()
        {
            _definition.PartialDescriptor.ShouldEqual(_descriptor.ToPartialSparkViewDescriptor());
        }

        [Test]
        public void caches_generated_view_definition()
        {
            ClassUnderTest.Create(_descriptor).ShouldBeTheSameAs(_definition);
        }
    }
}