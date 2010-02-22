using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;
using FubuMVC.Core.View.WebForms;
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
        private InMemoryFubuRequest _request;

        [SetUp]
        public void SetUp()
        {
            _mocks = new MockRepository();
            _view = MockRepository.GenerateStub<IFubuPage>();
            _partialView = MockRepository.GenerateStub<IFubuPage>();
            _request = new InMemoryFubuRequest();
            _renderer = MockRepository.GenerateMock<IPartialRenderer>();

            _model = new TestModel();
            _partialModel = new PartialTestModel();

            _model.PartialModel = _partialModel;

            _request.Set(_model);

            _expression = new RenderPartialExpression<TestModel>(_view, _renderer, _request);
            _expression.Using<IFubuPage>(v => { _wasCalled = true; });
        }

        [Test]
        public void using_control_should_call_the_option_action_on_the_view()
        {
            _wasCalled.ShouldBeTrue();
        }

        [Test]
        public void a_call_to_For_should_result_in_only_one_render()
        {
            _renderer.Expect(r => r.Render<object>(null, (IFubuPage) null, null, "")).IgnoreArguments().Return("");
            _expression.For(m => m.PartialModel).ToString();
            _renderer.VerifyAllExpectations();
        }

        [Test]
        public void a_call_to_For_should_pass_the_correct_model_to_render()
        {
            var args = _renderer.CaptureArgumentsFor(r => r.Render<object>(null, (IFubuPage)null, null, ""), o => o.Return(""));
            _expression.For(m => m.PartialModel).ToString();
            args.ArgumentAt<PartialTestModel>(2).ShouldBeTheSameAs(_partialModel);
        }

        [Test]
        public void a_call_to_For_should_pass_the_correct_prefix_to_render()
        {
            var args = _renderer.CaptureArgumentsFor(r => r.Render<object>(null, (IFubuPage)null, null, ""), o => o.Return(""));
            _expression.For(m => m.PartialModel).ToString();
            args.ArgumentAt<string>(3).ShouldEqual("PartialModel");
        }

        [Test]
        public void a_call_to_For_without_prefix_should_not_render_a_prefix()
        {
            var args = _renderer.CaptureArgumentsFor(r => r.Render<object>(null, (IFubuPage)null, _model, ""), oncall => oncall.Return(""));
            _expression.For(m => m.PartialModel).WithoutPrefix().ToString();
            args.ArgumentAt<string>(3).ShouldEqual("");
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

            _renderer.Expect(r => r.Render((IFubuPage)null, _model, "")).Return("").Constraints(
                Is.Anything(),
                Is.Same(_partialModel),
                Is.NotNull());

            _renderer.Expect(r => r.Render((IFubuPage)null, _model, "")).Return("").Constraints(
                Is.Anything(),
                Is.Same(model2),
                Is.NotNull());

            _renderer.Expect(r => r.Render((IFubuPage)null, _model, "")).Return("").Constraints(
                Is.Anything(),
                Is.Same(model3),
                Is.NotNull());

            _renderer.Expect(r => r.Render((IFubuPage)null, _model, "")).Return("").IgnoreArguments();

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

        public class TestModel
        {
            public PartialTestModel PartialModel { get; set; }
            public PartialTestModel[] PartialModelArray { get; set; }
        }

        public class PartialTestModel
        {
            
        }
    }
}