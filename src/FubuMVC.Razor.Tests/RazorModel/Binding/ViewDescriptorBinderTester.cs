using FubuMVC.Core.View.Model;
using FubuMVC.Razor.RazorModel;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Razor.Tests.RazorModel.Binding
{
    public class ViewDescriptorBinderTester : InteractionContext<ViewDescriptorBinder<IRazorTemplate>>
    {
        protected override void beforeEach()
        {
            Services.Inject<ITemplateSelector<IRazorTemplate>>(new RazorTemplateSelector());
        }

        [Test]
        public void bind_partials()
        {
            var request = new BindRequest<IRazorTemplate>
            {
                Target = new Template("_partial.cshtml", "", "testing")
            };

            ClassUnderTest.CanBind(request).ShouldBeTrue();
        }

        [Test]
        public void does_not_bind_files_other_than_razor()
        {
            var request = new BindRequest<IRazorTemplate>
            {
                Target = new Template("_partial.html", "", "testing")
            };

            ClassUnderTest.CanBind(request).ShouldBeFalse();
        }

        [Test]
        public void does_not_bind_if_descriptor_is_already_set()
        {
            var request = new BindRequest<IRazorTemplate>
            {
                Target = new Template("Fubu.cshtml", "", "testing")                
            };

            request.Target.Descriptor = new ViewDescriptor<IRazorTemplate>(request.Target);

            ClassUnderTest.CanBind(request).ShouldBeFalse();
        }

        [Test]
        public void if_template_is_valid_for_binding_then_binder_can_be_applied()
        {
            var request = new BindRequest<IRazorTemplate>
            {
                Target = new Template("Fubu.cshtml", "", "testing")
            };

            ClassUnderTest.CanBind(request).ShouldBeTrue();
        }

        [Test]
        public void assign_descriptor_as_an_instance_of_viewdescriptor()
        {
            var request = new BindRequest<IRazorTemplate>
            {
                Target = new Template("Fubu.cshtml", "", "testing"),
            };

            ClassUnderTest.Bind(request);
            request.Target.Descriptor.ShouldBeOfType<ViewDescriptor<IRazorTemplate>>();
        }
    }
}
