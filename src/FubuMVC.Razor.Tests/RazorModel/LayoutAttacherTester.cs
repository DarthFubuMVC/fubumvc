using FubuCore;
using FubuMVC.Core.View.Model;
using FubuMVC.Razor.RazorModel;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Razor.Tests.RazorModel
{
    [TestFixture]
    public class LayoutAttacherTester //: InteractionContext<MasterAttacher<RazorTemplate>>
    {
//        private AttachRequest<RazorTemplate> _request;
//        private Parsing _parsing;
//        private RazorTemplate _template;

//        protected override void beforeEach()
//        {
//            Assert.Fail("Probably redo.");
////            _template = new RazorTemplate("b/a.cshtml", "b", "c");
////            _template.ViewModel = typeof (ProductModel);
////
////            
////            _parsing = new Parsing
////            {
////                Master = "application",
////                ViewModelType = _template.ViewModel.FullName
////            };
////
////            _request = new AttachRequest<RazorTemplate>
////            {
////                Template = _template,
////                Logger = MockFor<ITemplateLogger>()
////            };
//
//        }

        [Test]
        public void JustRedo()
        {
            Assert.Fail("Redo");
        }

        [Test]
        public void if_template_is_valid_for_attachment_then_attacher_is_applied()
        {
            //ClassUnderTest.CanAttach(_request).ShouldBeTrue();
        }

        [Test]
        public void if_explicit_empty_master_then_binder_is_not_applied()
        {
//            _parsing.Master = string.Empty;
//            ClassUnderTest.CanAttach(_request).ShouldBeFalse();
        }

        /*
        [Test]
        public void if_view_model_type_is_null_and_master_is_invalid_then_binder_is_not_applied_1()
        {
            _template.ViewModel = null;            
            _parsing.Master = null;

            ClassUnderTest.CanAttach(_request).ShouldBeFalse();
        }

        [Test]
        public void if_view_model_type_is_null_and_master_is_invalid_then_binder_is_not_applied_2()
        {
            _template.ViewModel = null;
            _parsing.Master = "";

            ClassUnderTest.CanAttach(_request).ShouldBeFalse();
        }

        [Test]
        public void if_master_is_already_set_binder_is_not_applied()
        {
            _template.Master = MockFor<RazorTemplate>();
            ClassUnderTest.CanAttach(_request).ShouldBeFalse();
        }

        [Test]
        public void does_not_bind_partials()
        {
            _request.Template = new RazorTemplate("b/_partial.cshtml", "b", "c");
            ClassUnderTest.CanAttach(_request).ShouldBeFalse();
        }

        [Test]
        public void when_master_is_not_set_fallback_is_used_by_locator()
        {
            _parsing.Master = null;
            ClassUnderTest.Attach(_request);

            MockFor<ISharedTemplateLocator<RazorTemplate>>()
                .AssertWasCalled(x => x.LocateMaster(ClassUnderTest.MasterName, _template));
        }

        [Test]
        public void when_master_is_set_it_is_used_by_locator()
        {
            ClassUnderTest.Attach(_request);
            MockFor<ISharedTemplateLocator<RazorTemplate>>()
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
            _template.Master.ShouldEqual(MockFor<RazorTemplate>());
        }

        [Test]
        public void when_master_is_found_it_is_logged()
        {
            master_is_found();
            ClassUnderTest.Attach(_request);
            verify_log_contains("found at");
        }

        [Test]
        public void if_template_is_default_master_and_layout_is_itself_then_attacher_is_not_applied()
        {
            ((RazorTemplate)_template).FilePath = "One/Shared/{0}.cshtml".ToFormat(_parsing.Master);
            ClassUnderTest.CanAttach(_request).ShouldBeFalse();
        }


        private void verify_log_contains(string snippet)
        {
            MockFor<ITemplateLogger>()
                .AssertWasCalled(x => x.Log(Arg<RazorTemplate>.Is.Equal(_template), Arg<string>.Matches(s => s.Contains(snippet))));            
        }

        private void master_is_found()
        {
            MockFor<ISharedTemplateLocator<RazorTemplate>>()
                .Stub(x => x.LocateMaster(_parsing.Master, _template))
                .Return(MockFor<RazorTemplate>());            
        }
         */
    }

    public class ProductModel{}
}