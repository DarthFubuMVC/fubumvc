using System.Linq;
using FubuMVC.Core.View.Model;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests.SparkModel
{
    public class BindingsAttacherTester : InteractionContext<BindingsAttacher>
    {
        private ITemplate _template;
        private IAttachRequest<ITemplate> _request;
        private TemplateRegistry<ITemplate> _templates;
        private ViewDescriptor _viewDescriptor;

        protected override void beforeEach()
        {
            _templates = new TemplateRegistry<ITemplate>();
            _viewDescriptor = new ViewDescriptor(_template);
            _template = new Template("/App/Views/Fubu.spark", "/App/Views", FubuSparkConstants.HostOrigin)
            {
                Descriptor = _viewDescriptor                                
            };

            _templates.Add(_template);
            _templates.AddRange(Enumerable.Range(1, 5).Select(x => MockRepository.GenerateMock<ITemplate>()));

            _request = new AttachRequest<ITemplate>
            {
                Template = _template,
                Logger = MockFor<ITemplateLogger>(),
            };

            MockFor<ISharedTemplateLocator>()
                .Expect(x => x.LocateBindings(ClassUnderTest.BindingsName, _template))
                .Return(_templates);

            Container.Inject<ITemplateRegistry<ITemplate>>(_templates);
        }

        [Test]
        public void binds_templates_with_viewdescriptor_and_view_model_type()
        {
            ClassUnderTest.CanAttach(_request).ShouldBeTrue();
        }

        [Test]
        public void add_each_binding_to_the_descriptor()
        {
            ClassUnderTest.Attach(_request);

            _viewDescriptor.Bindings.ShouldEqual(_templates);
            MockFor<ISharedTemplateLocator>().VerifyAllExpectations();
        }

        [Test]
        public void does_not_bind_templates_with_no_viewdescriptor()
        {
            _template.Descriptor = new NulloDescriptor();
            ClassUnderTest.CanAttach(_request).ShouldBeFalse();
        }

        [Test]
        public void does_not_attempt_add_bindings_if_prior_processed()
        {
            _viewDescriptor.AddBinding(MockFor<ITemplate>());
            ClassUnderTest.CanAttach(_request).ShouldBeFalse();
        }

        [Test]
        public void logger_is_used()
        {
            ClassUnderTest.Attach(_request); 
            MockFor<ITemplateLogger>().AssertWasCalled(x => 
                x.Log(Arg.Is(_template), Arg<string>.Is.Anything), 
                x => x.Repeat.Times(_templates.Count));
        }
    }
}