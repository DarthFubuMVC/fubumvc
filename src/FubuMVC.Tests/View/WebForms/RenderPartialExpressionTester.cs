using System.Security.Principal;
using System.Threading;
using FubuMVC.Core;
using FubuMVC.Core.Security;
using FubuMVC.Core.UI.Partials;
using FubuMVC.Core.UI.Tags;
using FubuMVC.Core.View;
using FubuMVC.Core.View.WebForms;
using HtmlTags;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace FubuMVC.Tests.View.WebForms
{
    [TestFixture]
    public class RenderPartialExpressionTester
    {
        private MockRepository _mocks;
        private IFubuPage _view;
        private IFubuPage _partialView;
        private RenderPartialExpression<TestModel> _expression;
        private IPartialRenderer _renderer;
        private bool _wasCalled = false;
        private TestModel _model;
        private PartialTestModel _partialModel;
        private ITagGenerator<TestModel> _tagGenerator;
        private IEndpointService _endpointService;

        [SetUp]
        public void SetUp()
        {
            _mocks = new MockRepository();
            _view = MockRepository.GenerateStub<IFubuPage>();
            _partialView = MockRepository.GenerateStub<IFubuPage>();
            _renderer = MockRepository.GenerateMock<IPartialRenderer>();
            _tagGenerator = MockRepository.GenerateMock<ITagGenerator<TestModel>>();
            _endpointService = MockRepository.GenerateMock<IEndpointService>();
            _model = new TestModel();
            _partialModel = new PartialTestModel();

            _model.PartialModel = _partialModel;

            _expression = new RenderPartialExpression<TestModel>(_model, _view, _renderer,_tagGenerator, _endpointService);
            _expression.Using<IFubuPage>(v => { _wasCalled = true; });
        }

        [Test]
        public void when_rendering_a_partial_and_the_user_has_access_to_the_role()
        {
            PrincipalRoles.Current = new GenericPrincipal(Thread.CurrentPrincipal.Identity, new[] { "foo" });
            _expression.RequiresAccessTo("foo", "bar");
            _expression.For(x => new TestModel().PartialModel);
            _expression.ToString().ShouldNotEqual(string.Empty);
            PrincipalRoles.Current = null;
        }

        [Test]
        public void when_rendering_a_partial_and_the_user_does_not_have_access_to_the_role()
        {
            PrincipalRoles.Current = new GenericPrincipal(Thread.CurrentPrincipal.Identity, new[] { "foo" });
            _expression.RequiresAccessTo("bar");            
            _expression.ToString().ShouldEqual(string.Empty);
            PrincipalRoles.Current = null;
        }

        [Test]
        public void using_control_should_call_the_option_action_on_the_view()
        {
            _wasCalled.ShouldBeTrue();
        }

        [Test]
        public void if_endpoint_service_returns_false_should_not_render_the_partial()
        {
            _endpointService.Stub(x => x.EndpointFor<TestModel>(y => y.NoAccess())).IgnoreArguments().Return(new Endpoint{IsAuthorized = false});
            _expression.RequiresAccessTo<TestModel>(x => x.NoAccess());
            _expression.ToString().ShouldEqual(string.Empty);
        }

        [Test]
        public void if_enpoint_service_returns_false_at_any_point_should_not_render_the_partial()
        {
            //two enpoints, one with access and one without access, make sure we don't render even if the last call has access.
            _endpointService.Stub(x => x.EndpointFor<TestModel>(y => y.Access())).IgnoreArguments().Return(new Endpoint { IsAuthorized = true }).Repeat.Once();
            _endpointService.Stub(x => x.EndpointFor<TestModel>(y => y.NoAccess())).IgnoreArguments().Return(new Endpoint {IsAuthorized = false}).Repeat.Once();
            
            _expression.For(x => new TestModel().PartialModel).RequiresAccessTo<TestModel>(x => x.Access()).RequiresAccessTo<TestModel>(x => x.NoAccess()).ToString().ShouldEqual(string.Empty);
        }

        [Test]
        public void if_endpoint_service_returns_true_should_render_the_partial()
        {
            _endpointService.Stub(x => x.EndpointFor<TestModel>(y => y.Access())).IgnoreArguments().Return(new Endpoint { IsAuthorized = true });
            _expression.For(x=> new TestModel().PartialModel).RequiresAccessTo<TestModel>(x => x.Access());
            _expression.ToString().ShouldNotEqual(string.Empty);            
        }

        [Test]
        public void a_call_to_For_should_result_in_only_one_render()
        {
            _renderer.Expect(r => r.Render<PartialTestModel>(null, (IFubuPage)null, null, "")).IgnoreArguments().Return("");
            _expression.For(m => m.PartialModel).ToString();
            _renderer.VerifyAllExpectations();
        }

        [Test]
        public void a_call_to_For_should_pass_the_correct_model_to_render()
        {
            var args = _renderer.CaptureArgumentsFor(r => r.Render<PartialTestModel>(null, (IFubuPage)null, null, ""), o => o.Return(""));
            _expression.For(m => m.PartialModel).ToString();
            args.ArgumentAt<PartialTestModel>(2).ShouldBeTheSameAs(_partialModel);
        }

        [Test]
        public void a_call_to_For_should_pass_the_correct_prefix_to_render()
        {
            var args = _renderer.CaptureArgumentsFor(r => r.Render<PartialTestModel>(null, (IFubuPage)null, null, ""), o => o.Return(""));
            _expression.For(m => m.PartialModel).ToString();
            args.ArgumentAt<string>(3).ShouldEqual("PartialModel");
        }


        [Test]
        public void a_call_to_ForEachOf_should_result_in_multiple_renders()
        {
            _model.PartialModelArray = new[] {_partialModel, _partialModel, _partialModel};

            _renderer.Expect(r => r.Render<PartialTestModel>((IFubuPage)null, null, "")).Return("").IgnoreArguments().Repeat.Times(3, 99);

            _expression.ForEachOf(m => m.PartialModelArray).ToString();
            _renderer.VerifyAllExpectations();
        }

        [Test]
        public void a_call_to_ForEachOf_should_pass_the_correct_models_to_each_render_call()
        {
            var model2 = new PartialTestModel();
            var model3 = new PartialTestModel();

            _model.PartialModelArray = new[] { _partialModel, model2, model3 };

            _renderer.Expect(r => r.Render((IFubuPage)null, _partialModel, "PartialModelArray")).Return("test").Constraints(
                Is.Anything(),
                Is.Same(_partialModel),
                Is.NotNull());

            _renderer.Expect(r => r.Render((IFubuPage)null, model2, "PartialModelArray")).Return("").Constraints(
                Is.Anything(),
                Is.Same(model2),
                Is.NotNull());

            _renderer.Expect(r => r.Render((IFubuPage)null, model3, "PartialModelArray")).Return("").Constraints(
                Is.Anything(),
                Is.Same(model3),
                Is.NotNull());

            _expression.ForEachOf(m => m.PartialModelArray).ToString();

            _renderer.VerifyAllExpectations();

        }

        [Test]
        public void a_call_to_ForEachOf_with_pagination_should_pass_the_correct_prefix_to_each_render_call()
        {
            var model2 = new PartialTestModel();
            var model3 = new PartialTestModel();

            _model.PartialModelArray = new[] { _partialModel, model2, model3 };

            _renderer.Expect(r => r.Render((IFubuPage)null, _partialModel, "_0PartialModelArray_PartialTestModel")).Return("");
            _renderer.Expect(r => r.Render((IFubuPage)null, model2, "_1PartialModelArray_PartialTestModel")).Return("");
            _renderer.Expect(r => r.Render((IFubuPage)null, model3, "_2PartialModelArray_PartialTestModel")).Return("");

            _renderer.Expect(r => r.Render((IFubuPage)null, _model, "")).Return("").IgnoreArguments();


            _expression.ForEachOf(m => m.PartialModelArray).ToString();
        }

        [Test]
        public void a_call_to_ForEachOf_with_pagination_should_append_a_blank_template_to_the_end()
        {
            var model2 = new PartialTestModel();
            var model3 = new PartialTestModel();

            _model.PartialModelArray = new[] { _partialModel, model2, model3 };

            _renderer.Expect(r => r.Render((IFubuPage)null, _model, "")).Return("").IgnoreArguments().Repeat.Times(3);

            _renderer.Expect(r => r.Render((IFubuPage)null, _model, "_blankPartialModelArray")).Return("").IgnoreArguments();
            
            _expression.ForEachOf(m => m.PartialModelArray).ToString();
        }

        [Test]
        public void a_call_to_ForEachOf_should_generate_beforePartialTag_and_afterPartialTag()
        {
            var model2 = new PartialTestModel();
            var model3 = new PartialTestModel();

            _model.PartialModelArray = new[] { _partialModel, model2, model3 };

            _tagGenerator.Expect(c => c.BeforePartial(null)).IgnoreArguments().Return(new NoTag());

            _tagGenerator.Expect(c => c.AfterPartial(null)).IgnoreArguments().Return(new NoTag());
            _expression.ForEachOf(m => m.PartialModelArray).ToString();

            _tagGenerator.VerifyAllExpectations();
        }

        [Test]
        public void a_call_to_ForEachOf_should_generate_beforeEachOfPartialTag_and_afterEachOfTag()
        {
            var model2 = new PartialTestModel();
            var model3 = new PartialTestModel();

            _model.PartialModelArray = new[] { _partialModel, model2, model3 };

            _tagGenerator.Expect(c => c.BeforeEachofPartial(null, 0, 3)).IgnoreArguments().Return(new NoTag());
            _tagGenerator.Expect(c => c.BeforeEachofPartial(null, 1, 3)).IgnoreArguments().Return(new NoTag());
            _tagGenerator.Expect(c => c.BeforeEachofPartial(null, 2, 3)).IgnoreArguments().Return(new NoTag());

            _tagGenerator.Expect(c => c.AfterEachofPartial(null, 0, 3)).IgnoreArguments().Return(new NoTag());
            _tagGenerator.Expect(c => c.AfterEachofPartial(null, 1, 3)).IgnoreArguments().Return(new NoTag());
            _tagGenerator.Expect(c => c.AfterEachofPartial(null, 2, 3)).IgnoreArguments().Return(new NoTag());

            _expression.ForEachOf(m => m.PartialModelArray).ToString();

            _tagGenerator.VerifyAllExpectations();
        }

        [Test]
        public void a_call_to_withoutListWrapper_should_not_render_output_before_on_foreeachof()
        {
            var model2 = new PartialTestModel();
            var model3 = new PartialTestModel();

            _model.PartialModelArray = new[] { _partialModel, model2, model3 };

            _tagGenerator.Expect(c => c.AfterPartial(null)).IgnoreArguments().Return(new NoTag());

            _expression.WithoutListWrapper().ForEachOf(m => m.PartialModelArray).ToString();

            _tagGenerator.AssertWasNotCalled(c => c.BeforePartial(null), b => b.IgnoreArguments());
            _tagGenerator.AssertWasNotCalled(c => c.AfterPartial(null), b => b.IgnoreArguments());
        }

        [Test]
        public void a_call_to_withoutItemWrapper_should_not_wrap_the_items()
        {
            var model2 = new PartialTestModel();
            var model3 = new PartialTestModel();

            _model.PartialModelArray = new[] { _partialModel, model2, model3 };

            _tagGenerator.Expect(c => c.AfterPartial(null)).IgnoreArguments().Return(new NoTag());

            _expression.WithoutItemWrapper().ForEachOf(m => m.PartialModelArray).ToString();

            _tagGenerator.AssertWasNotCalled(c => c.BeforeEachofPartial(null, 0, 3), b => b.IgnoreArguments().Repeat.Times(3));
            _tagGenerator.AssertWasNotCalled(c => c.AfterEachofPartial(null, 0, 3), b => b.IgnoreArguments().Repeat.Times(3));
        }

        public class TestModel
        {
            public PartialTestModel PartialModel { get; set; }
            public PartialTestModel[] PartialModelArray { get; set; }
            public void NoAccess(){}
            public void Access() {}
        }

        public class PartialTestModel
        {            
        }
    }    
}