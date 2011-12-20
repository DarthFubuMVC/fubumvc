using System.Linq;
using FubuCore;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests.SparkModel
{
    public class BindingsAttacherTester : InteractionContext<BindingsAttacher>
    {
        private ITemplate _template;
        private IAttachRequest _request;
        private TemplateRegistry _templates;

        protected override void beforeEach()
        {
            _templates = new TemplateRegistry();
            _template = new Template("/App/Views/Fubu.spark", "/App/Views", FubuSparkConstants.HostOrigin)
            {
                Descriptor = new ViewDescriptor(_template)                                
            };

            _templates.Add(_template);
            _templates.AddRange(Enumerable.Range(1, 5).Select(x => MockRepository.GenerateMock<ITemplate>()));

            _request = new AttachRequest
            {
                Template = _template,
                Logger = MockFor<ISparkLogger>(),
            };

            MockFor<ISharedTemplateLocator>()
                .Expect(x => x.LocateBindings(ClassUnderTest.BindingsName, _template))
                .Return(_templates);

            Container.Inject<ITemplateRegistry>(_templates);
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

            _template.Descriptor.As<ViewDescriptor>().Bindings.ShouldEqual(_templates);
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
            _template.Descriptor.As<ViewDescriptor>().AddBinding(MockFor<ITemplate>());
            ClassUnderTest.CanAttach(_request).ShouldBeFalse();
        }

        [Test]
        public void logger_is_used()
        {
            MockFor<ISparkLogger>()
                .Expect(x => x.Log(Arg.Is(_template), Arg<string>.Is.Anything))
                .Repeat.Times(_templates.Count);

            ClassUnderTest.Attach(_request); 
            MockFor<ISparkLogger>().VerifyAllExpectations();
        }
    }
}