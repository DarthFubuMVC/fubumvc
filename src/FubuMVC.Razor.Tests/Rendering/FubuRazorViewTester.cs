using System.ComponentModel;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using FubuMVC.Razor.Rendering;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Razor.Tests.Rendering
{
    [TestFixture]
    public class FubuRazorViewTester : InteractionContext<StubView>
    {
        private IServiceLocator _serviceLocator;
        private IUrlRegistry _urlRegistry;

        protected override void beforeEach()
        {
            _serviceLocator = MockFor<IServiceLocator>();
            _urlRegistry = MockFor<IUrlRegistry>();

            ClassUnderTest.ServiceLocator.ShouldBeNull();
            ClassUnderTest.ServiceLocator = _serviceLocator;

            _serviceLocator.Stub(x => x.GetInstance(typeof (IUrlRegistry))).Return(_urlRegistry);
        }

        [Test]
        public void service_locator()
        {
            ClassUnderTest.ServiceLocator.ShouldEqual(_serviceLocator);
        }

        [Test]
        public void get_returns_a_cached_instance_from_the_service_locator()
        {
            var original = new ViewObject {Tag = "original"};
            var latest = new ViewObject {Tag = "latest"};

            _serviceLocator.Stub(x => x.GetInstance(typeof (ViewObject))).Return(original);
            var first = ClassUnderTest.Get<ViewObject>();
            _serviceLocator.BackToRecord(BackToRecordOptions.All);
            _serviceLocator.Replay(); 
            _serviceLocator.Stub(x => x.GetInstance(typeof(ViewObject))).Return(latest);
            var second = ClassUnderTest.Get<ViewObject>();

            first.ShouldEqual(original);
            second.ShouldEqual(original);
            first.Tag.ShouldEqual(original.Tag);
        }

        [Test]
        public void get_new_uses_the_service_locator_always()
        {
            var original = new ViewObject { Tag = "original" };
            var latest = new ViewObject { Tag = "latest" };

            _serviceLocator.Stub(x => x.GetInstance(typeof(ViewObject))).Return(original);
            var first = ClassUnderTest.GetNew<ViewObject>();
            _serviceLocator.BackToRecord(BackToRecordOptions.All);
            _serviceLocator.Replay();
            _serviceLocator.Stub(x => x.GetInstance(typeof(ViewObject))).Return(latest);
            var second = ClassUnderTest.GetNew<ViewObject>();

            first.ShouldEqual(original);
            second.ShouldEqual(latest);
            first.Tag.ShouldEqual(original.Tag);
            second.Tag.ShouldEqual(latest.Tag);
        }

        [Test]
        public void urls()
        {
            ClassUnderTest.Urls.ShouldEqual(_urlRegistry);
        }

        [Test]
        public void tag()
        {
            ClassUnderTest.Tag("div").ShouldNotBeNull().TagName().ShouldEqual("div");
        }

        [Test]
        public void raw_does_not_encode_value()
        {
            const string encodedValue = "&lt;div&gt;";
            var result = ClassUnderTest.Raw(encodedValue);
            result.ToString().ShouldEqual(encodedValue);
            result.ToHtmlString().ShouldEqual(encodedValue);
        }

        [Test]
        public void raw_handles_null_value()
        {
            var result = ClassUnderTest.Raw(null);
            result.ToString().ShouldEqual(null);
            result.ToHtmlString().ShouldEqual(null);
        }

        [Test]
        public void set_model_using_request()
        {
            var request = MockFor<IFubuRequest>();
            var model = new PersonViewModel {Name = "Mr. FubuRazor"};
            request.Expect(x => x.Has<PersonViewModel>()).Return(true);
            request.Expect(x => x.Get<PersonViewModel>()).Return(model);
            ClassUnderTest.SetModel(request);
            ClassUnderTest.Model.ShouldEqual(model);
            request.VerifyAllExpectations();
        }

        [Test]
        public void views_should_find_if_fubu_request_doesnt_have_the_model_type()
        {
            var request = MockFor<IFubuRequest>();
            var model = new PersonViewModel {Name = "Mr. FubuRazor"};
            request.Expect(x => x.Has<SharedViewModel>()).Return(false);
            request.Expect(x => x.Find<SharedViewModel>()).Return(new SharedViewModel[] {model});
            var sharedLayout = new SharedView();
            sharedLayout.SetModel(request);
            sharedLayout.Model.ShouldEqual(model);
            request.VerifyAllExpectations();
        }

        [Test]
        public void set_model_using_object_instance()
        {
            object model = new PersonViewModel { Name = "Mr. FubuRazor" };
            ClassUnderTest.SetModel(model);
            ClassUnderTest.Model.ShouldEqual(model);
        }

        [Test]
        public void set_model_using_model_instance()
        {
            var model = new PersonViewModel { Name = "Mr. FubuRazor" };
            ClassUnderTest.SetModel(model);
            ClassUnderTest.Model.ShouldEqual(model);
        }

        [Test]
        public void get_model_returns_the_view_model()
        {
            var model = new PersonViewModel { Name = "Mr. FubuRazor" };
            ClassUnderTest.SetModel(model);
            ClassUnderTest.GetModel().ShouldEqual(model);
        }

    }

    public class SharedViewModel
    {
    }

    public class PersonViewModel : SharedViewModel
    {
        public string Name { get; set; }
    }

    public class ViewObject 
    {
        public string Tag { get; set; }
    }

    public class SharedView : FubuRazorView<SharedViewModel>
    {
        public override void Execute()
        {
        }
    }
}
