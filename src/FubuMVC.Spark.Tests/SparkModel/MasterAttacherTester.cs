using FubuMVC.Core.View.Model;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests.SparkModel
{
    [TestFixture]
    public class MasterAttacherTester : InteractionContext<MasterAttacher<ITemplate>>
    {
        private AttachRequest<ITemplate> _request;
        private SparkDescriptor _viewDescriptor;
        private Parsing _parsing;
        private ITemplate _template;

        protected override void beforeEach()
        {
            _template = new Template("b/a.spark", "b", "c");
            _template.Descriptor = _viewDescriptor = new SparkDescriptor(_template)
            {
                ViewModel = typeof (ProductModel)
            };
            
            _parsing = new Parsing
            {
                Master = "application",
                ViewModelType = _viewDescriptor.ViewModel.FullName
            };

            _request = new AttachRequest<ITemplate>
            {
                Template = _template,
                Logger = MockFor<ITemplateLogger>()
            };

            MockFor<IParsingRegistrations<ITemplate>>().Expect(x => x.ParsingFor(_template)).Return(_parsing);
        }

        [Test]
        public void if_template_is_valid_for_attachment_then_attacher_is_applied()
        {
            ClassUnderTest.CanAttach(_request).ShouldBeTrue();
        }

        [Test]
        public void if_explicit_empty_master_then_binder_is_not_applied()
        {
            _parsing.Master = string.Empty;
            ClassUnderTest.CanAttach(_request).ShouldBeFalse();
        }

        [Test]
        public void if_descriptor_is_not_viewdescriptor_then_binder_is_not_applied()
        {
            _template.Descriptor = new NulloDescriptor();
            ClassUnderTest.CanAttach(_request).ShouldBeFalse();
        }

        [Test]
        public void if_view_model_type_is_null_and_master_is_invalid_then_binder_is_not_applied_1()
        {
            _viewDescriptor.ViewModel = null;            
            _parsing.Master = null;

            ClassUnderTest.CanAttach(_request).ShouldBeFalse();
        }

        [Test]
        public void if_view_model_type_is_null_and_master_is_invalid_then_binder_is_not_applied_2()
        {
            _viewDescriptor.ViewModel = null;
            _parsing.Master = "";

            ClassUnderTest.CanAttach(_request).ShouldBeFalse();
        }

        [Test]
        public void if_master_is_already_set_binder_is_not_applied()
        {
            _viewDescriptor.Master = MockFor<ITemplate>();
            ClassUnderTest.CanAttach(_request).ShouldBeFalse();
        }

        [Test]
        public void does_not_bind_partials()
        {
            _request.Template = new Template("b/_partial.spark", "b", "c");
            ClassUnderTest.CanAttach(_request).ShouldBeFalse();
        }

        [Test]
        public void when_master_is_not_set_fallback_is_used_by_locator()
        {
            _parsing.Master = null;
            ClassUnderTest.Attach(_request);

            MockFor<ISharedTemplateLocator<ITemplate>>()
                .AssertWasCalled(x => x.LocateMaster(ClassUnderTest.MasterName, _template));
        }

        [Test]
        public void when_master_is_set_it_is_used_by_locator()
        {
            ClassUnderTest.Attach(_request);
            MockFor<ISharedTemplateLocator<ITemplate>>()
                .AssertWasCalled(x => x.LocateMaster(_parsing.Master, _template));
        }

        [Test]
        public void when_no_master_is_found_it_is_logged()
        {
            ClassUnderTest.Attach(_request);
            verify_log_contains("not found");
        }

        [Test]
        public void when_master_is_found_it_is_set_on_view_descriptor()
        {
            master_is_found();
            ClassUnderTest.Attach(_request);
            _viewDescriptor.Master.ShouldEqual(MockFor<ITemplate>());
        }

        [Test]
        public void when_master_is_found_it_is_logged()
        {
            master_is_found();
            ClassUnderTest.Attach(_request);
            verify_log_contains("found at");
        }

        [Test]
        public void if_template_is_default_master_then_attacher_is_not_applied()
        {
            ((Template)_template).FilePath = "b/" + _parsing.Master + ".cshtml";
            ClassUnderTest.CanAttach(_request).ShouldBeFalse();
        }


        private void verify_log_contains(string snippet)
        {
            MockFor<ITemplateLogger>()
                .AssertWasCalled(x => x.Log(Arg<ITemplate>.Is.Equal(_template), Arg<string>.Matches(s => s.Contains(snippet))));            
        }

        private void master_is_found()
        {
            MockFor<ISharedTemplateLocator<ITemplate>>()
                .Stub(x => x.LocateMaster(_parsing.Master, _template))
                .Return(MockFor<ITemplate>());            
        }
    }
}