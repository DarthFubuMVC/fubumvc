using System;
using System.IO;
using System.Web.UI;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Activation;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.WebForms.Testing
{
    [TestFixture]
    public class PartialRendererTester
    {
        private IWebFormsControlBuilder _builder;
        private InMemoryFubuRequest _request;

        [SetUp]
        public void SetUp()
        {
            _builder = MockRepository.GenerateMock<IWebFormsControlBuilder>();
            _request = new InMemoryFubuRequest();
        }

        [Test]
        public void should_throw_if_type_is_not_a_control()
        {
            typeof(InvalidOperationException).ShouldBeThrownBy(
                () => new PartialRenderer(_builder, null, null).CreateControl(typeof(string)));
        }

        [Test]
        public void should_throw_if_type_is_not_a_IFubuPage()
        {
            typeof(InvalidOperationException).ShouldBeThrownBy(
                () => new PartialRenderer(_builder, null, null).CreateControl(typeof(Page)));
        }


        [Test]
        public void should_create_control_from_virtual_path()
        {
            _builder.Expect(b => b.LoadControlFromVirtualPath("~/TestControl.ascx", typeof(TestControl))).Return(
                new TestControl());

            new PartialRenderer(_builder, null, new InMemoryFubuRequest()).CreateControl(typeof(TestControl));

            _builder.VerifyAllExpectations();
        }

        [Test]
        public void should_execute_the_control_rendering_when_rendering()
        {
            _builder.Stub(b => b.LoadControlFromVirtualPath(null, null))
                .IgnoreArguments()
                .Return(new TestControl());

            _builder.Expect(b => b.ExecuteControl(null, null)).IgnoreArguments();

            _request.Set(new TestViewModel());

            new PartialRenderer(_builder, new StubActivator(), new InMemoryFubuRequest())
                .Render(new TestView(), typeof(TestControl), new TestControlViewModel(), "");

            _builder.VerifyAllExpectations();
        }
    }

    public class StubActivator : IPageActivator
    {
        public void Activate(IFubuPage page)
        {
        }
    }

    [TestFixture]
    public class PartialRenderer_setting_parent_page
    {
        private IWebFormsControlBuilder _builder;
        private SpecificationExtensions.CapturingConstraint _executeCatcher;
        private TestView _parentView;
        private PartialRenderer _renderer;
        private InMemoryFubuRequest _request;

        [SetUp]
        public void SetUp()
        {
            _builder = MockRepository.GenerateMock<IWebFormsControlBuilder>();
            _request = new InMemoryFubuRequest();

            _builder.Stub(b => b.LoadControlFromVirtualPath(null, null))
                   .IgnoreArguments()
                   .Return(new TestControl());

            _executeCatcher = _builder.CaptureArgumentsFor(b => b.ExecuteControl(null, null));

            _parentView = new TestView();
            _renderer = new PartialRenderer(_builder, new StubActivator(), _request);
        }

        [Test]
        public void should_set_the_parent_page()
        {
            _renderer.Render(_parentView, typeof(TestControl), new TestControlViewModel(), "");

            var control = _executeCatcher.First<Page>().Controls[0].ShouldBeOfType<TestControl>();
            ((INeedToKnowAboutParentPage)control).ParentPage.ShouldBeTheSameAs(_parentView);
        }

        [Test]
        public void should_set_the_parent_page_if_no_parent_is_specified()
        {
            _renderer.Render(new TestControl(), new TestControlViewModel(), "");

            var control = _executeCatcher.First<Page>().Controls[0].ShouldBeOfType<TestControl>();
            ((INeedToKnowAboutParentPage)control).ParentPage.ShouldBeOfType<Page>();
        }
    }

    [TestFixture]
    public class PartialRender_setting_model_in_FubuRequest : InteractionContext<PartialRenderer>
    {
        [Test]
        public void should_set_the_model_in_the_FubuRequest()
        {
            var viewModel = new TestControlViewModel();
            ClassUnderTest.Render(new TestControl(), viewModel, "", new StringWriter());

            MockFor<IFubuRequest>().AssertWasCalled(r => r.Set(viewModel.GetType(), viewModel));
        }
    }

    [TestFixture]
    public class PartialRender_when_view_model_doesnt_already_exist_in_request : InteractionContext<PartialRenderer>
    {
        private InMemoryFubuRequest _request;

        protected override void beforeEach()
        {
            _request = new InMemoryFubuRequest();
            Services.Inject<IFubuRequest>(_request);
        }

        [Test]
        public void should_clear_the_model_after_rendering()
        {
            ClassUnderTest.Render(new TestControl(), new TestControlViewModel(), "", new StringWriter());

            _request.Has(typeof(TestControlViewModel)).ShouldBeFalse();
        }
    }

    [TestFixture]
    public class PartialRender_when_view_model_already_exists_in_request : InteractionContext<PartialRenderer>
    {
        private InMemoryFubuRequest _request;
        private TestControlViewModel _viewModel;

        protected override void beforeEach()
        {
            _viewModel = new TestControlViewModel();
            _request = new InMemoryFubuRequest();
            _request.Set(_viewModel);
            Services.Inject<IFubuRequest>(_request);
        }

        [Test]
        public void should_not_clear_the_model_after_rendering()
        {
            ClassUnderTest.Render(new TestControl(), _viewModel, "", new StringWriter());

            _request.Has(typeof(TestControlViewModel)).ShouldBeTrue();
        }
    }


    public class TestViewModel
    {
        public bool BoolProperty { get; set; }
        public string StringProperty { get; set; }
        public int IntProperty { get; set; }
    }

    

    public class TestView : FubuPage<TestViewModel>
    {
    }

    public class TestControlViewModel
    {
        public string Name { get; set; }
    }

    public class TestControl : FubuControl<TestControlViewModel>
    {
    }
}