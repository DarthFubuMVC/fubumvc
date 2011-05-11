using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests.SparkModel.Binding
{
    public class ReachableBindingsBinderTester : InteractionContext<ReachableBindingsBinder>
    {
        [Test]
        public void binds_templates_with_viewdescriptor()
        {
            var template = new Template("Fubu.spark", "", "Package");
            template.Descriptor = new ViewDescriptor(template);
            var request = new BindRequest { Target = template};

            ClassUnderTest.CanBind(request).ShouldBeTrue();
        }

        [Test]
        public void does_not_bind_templates_with_nullodescriptor()
        {
            var template = new Template("Fubu.spark", "", "Package");
            var request = new BindRequest { Target = template };

            ClassUnderTest.CanBind(request).ShouldBeFalse();
        }

        // TODO : Test Bind
    }
}