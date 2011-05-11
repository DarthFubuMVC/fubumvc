using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests.SparkModel.Binding
{
    public class ViewDescriptorBinderTester : InteractionContext<ViewDescriptorBinder>
    {
        [Test]
        public void does_not_bind_partials()
        {
            var request = new BindRequest
            {
                Target = new Template("_partial.spark", "", "testing")
            };

            ClassUnderTest.CanBind(request).ShouldBeFalse();
        }

        [Test]
        public void does_not_bind_files_other_than_spark()
        {
            var request = new BindRequest
            {
                Target = new Template("_partial.html", "", "testing")
            };

            ClassUnderTest.CanBind(request).ShouldBeFalse();
        }

        [Test]
        public void does_not_bind_templates_with_no_view_model()
        {
            var request = new BindRequest
            {
                Target = new Template("Fubu.spark", "", "testing")                
            };

            ClassUnderTest.CanBind(request).ShouldBeFalse();
        }

        [Test]
        public void binds_valid_templates_with_view_model()
        {
            var request = new BindRequest
            {
                Target = new Template("Fubu.spark", "", "testing"),
                ViewModelType = "SomeModel"
            };

            ClassUnderTest.Bind(request);
            request.Target.Descriptor.ShouldBeOfType<ViewDescriptor>();
        }
    }
}
