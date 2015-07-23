using System;
using FubuCore;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using FubuMVC.Spark.Rendering;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.IntegrationTesting.Views.Spark.Rendering
{
    public class MySparkView : FubuSparkView<PersonViewModel>
    {
        public override void Render()
        {
            throw new NotImplementedException();
        }

        public override Guid GeneratedViewId
        {
            get { throw new NotImplementedException(); }
        }
    }

    [TestFixture]
    public class FubuSparkViewTester : InteractionContext<MySparkView>
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
            ClassUnderTest.ServiceLocator.ShouldBe(_serviceLocator);
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

            first.ShouldBe(original);
            second.ShouldBe(original);
            first.Tag.ShouldBe(original.Tag);
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

            first.ShouldBe(original);
            second.ShouldBe(latest);
            first.Tag.ShouldBe(original.Tag);
            second.Tag.ShouldBe(latest.Tag);
        }

        [Test]
        public void site_resource()
        {
            ClassUnderTest.SiteResource = x => "App/{0}".ToFormat(x);
            ClassUnderTest.SiteResource("Views/Home/Home.spark").ShouldBe("App/Views/Home/Home.spark");
        }

        [Test]
        public void urls()
        {
            ClassUnderTest.Urls.ShouldBe(_urlRegistry);
        }

        [Test]
        public void tag()
        {
            ClassUnderTest.Tag("div").ShouldNotBeNull().TagName().ShouldBe("div");
        }

        [Test]
        public void h_uses_servicelocator_to_get_htmlencoder()
        {
            _serviceLocator.Expect(x => x.GetInstance(typeof (IHtmlEncoder))).Return(new DefaultHtmlEncoder());
            ClassUnderTest.H("<div>").ShouldBe("&lt;div&gt;");
            _serviceLocator.VerifyAllExpectations();
        }

        [Test]
        public void html_does_not_encode_value()
        {
            const string encodedValue = "&lt;div&gt;";
            var result = ClassUnderTest.HTML(encodedValue);
            result.ToString().ShouldBe(encodedValue);
            result.ToHtmlString().ShouldBe(encodedValue);
        }

        [Test]
        public void html_handles_null_value()
        {
            var result = ClassUnderTest.HTML(null);
            result.ToString().ShouldBe(null);
            result.ToHtmlString().ShouldBe(null);
        }

        [Test]
        public void set_model_using_request()
        {
            var request = MockFor<IFubuRequest>();
            var model = new PersonViewModel {Name = "Mr. FubuSpark"};
            request.Expect(x => x.Get<PersonViewModel>()).Return(model);
            ClassUnderTest.SetModel(request);
            ClassUnderTest.Model.ShouldBe(model);
            request.VerifyAllExpectations();
        }

        [Test]
        public void set_model_using_object_instance()
        {
            object model = new PersonViewModel { Name = "Mr. FubuSpark" };
            ClassUnderTest.SetModel(model);
            ClassUnderTest.Model.ShouldBe(model);
        }

        [Test]
        public void set_model_using_model_instance()
        {
            var model = new PersonViewModel { Name = "Mr. FubuSpark" };
            ClassUnderTest.SetModel(model);
            ClassUnderTest.Model.ShouldBe(model);
        }

        [Test]
        public void get_model_returns_the_view_model()
        {
            var model = new PersonViewModel { Name = "Mr. FubuSpark" };
            ClassUnderTest.SetModel(model);
            ClassUnderTest.GetModel().ShouldBe(model);
        }

    }

    public class PersonViewModel
    {
        public string Name { get; set; }
    }

    public class ViewObject 
    {
        public string Tag { get; set; }
    }

}
