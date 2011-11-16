using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests.SparkModel.Binding
{
    public class ReachableBindingsBinderTester : InteractionContext<ReachableBindingsBinder>
    {
        private ISharedTemplateLocator _sharedTemplateLocator;
        private ITemplate _template;
        private IBindRequest _request;
        private IList<ITemplate> _bindings;
        private ISparkLogger _logger;

        protected override void beforeEach()
        {
            var repo = new MockRepository();
            
            _bindings = new List<ITemplate>();
            _bindings.AddRange(Enumerable.Range(1, 5).Select(x => repo.DynamicMock<ITemplate>()));

            _logger = MockFor<ISparkLogger>();
            _sharedTemplateLocator = MockFor<ISharedTemplateLocator>();

            _template = MockFor<ITemplate>();
            _template.Stub(x => x.Descriptor).PropertyBehavior();
            _template.Stub(x => x.FilePath).Return("Fubu.spark");
            _template.Stub(x => x.RootPath).Return("/App/Views");
            _template.Stub(x => x.Origin).Return("Host");            
            _template.Descriptor = new ViewDescriptor(_template);

            ClassUnderTest.BindingsName = "bindings";

            _request = new BindRequest
            {
                TemplateRegistry = new TemplateRegistry(new[] { _template }.Union(_bindings)),
                Target = _template,
                Parsing = new Parsing {ViewModelType = typeof (ProductModel).FullName},
                Logger = _logger,
            };

            _sharedTemplateLocator
                .Expect(x => x.LocateBindings(ClassUnderTest.BindingsName, _template, _request.TemplateRegistry))
                .Return(_bindings);
            
            _logger
                .Expect(x => x.Log(Arg.Is(_template), Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                .Repeat.Times(_bindings.Count);
        }

        [Test]
        public void binds_templates_with_viewdescriptor_and_view_model_type()
        {
            ClassUnderTest.CanBind(_request).ShouldBeTrue();
        }

        [Test]
        public void add_each_binding_to_the_descriptor()
        {
            ClassUnderTest.Bind(_request);
            _sharedTemplateLocator.VerifyAllExpectations();
            _template.Descriptor.As<ViewDescriptor>().Bindings.ShouldEqual(_bindings);

        }

        [Test]
        public void does_not_bind_templates_with_no_viewdescriptor()
        {
            _template.Descriptor = new NulloDescriptor();
            ClassUnderTest.CanBind(_request).ShouldBeFalse();
        }

        [Test]
        public void does_not_attempt_add_bindings_if_prior_processed()
        {
            _template.Descriptor.As<ViewDescriptor>().AddBinding(MockFor<ITemplate>());            
            ClassUnderTest.CanBind(_request).ShouldBeFalse();
        }

        [Test]
        public void logger_is_used()
        {
            ClassUnderTest.Bind(_request);
            _logger.VerifyAllExpectations();
        }
    }
}