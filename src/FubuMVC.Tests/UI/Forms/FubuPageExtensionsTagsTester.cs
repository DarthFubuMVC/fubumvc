using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Routing;
using FubuCore.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using FubuMVC.StructureMap;
using FubuMVC.UI;
using FubuMVC.UI.Configuration;
using FubuMVC.UI.Tags;
using HtmlTags;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;

namespace FubuMVC.Tests.UI.Forms
{
    [TestFixture]
    public class when_calling_link_to
    {
        private IFubuPage _page;
        private IUrlRegistry _urls;
        private InputModel _model;

        [SetUp]
        public void SetUp()
        {
            _page = MockRepository.GenerateMock<IFubuPage>();
            _urls = MockRepository.GenerateStub<IUrlRegistry>();
            _model = new InputModel();
            _urls.Stub(u => u.UrlFor(Arg<InputModel>.Is.NotNull)).Return("some url");
            _page.Expect(p => p.Urls).Return(_urls);
        }

        [Test]
        public void should_return_html_tag()
        {
            HtmlTag tag = _page.LinkTo(_model);
            tag.Attr("href").ShouldEqual("some url");
            _urls.AssertWasCalled(u => u.UrlFor(_model));
        }

        [Test]
        public void should_return_html_tag_of_new_model()
        {
            HtmlTag tag = _page.LinkTo<InputModel>();
            tag.Attr("href").ShouldEqual("some url");
            _urls.AssertWasNotCalled(u => u.UrlFor(_model));
            _urls.AssertWasCalled(u => u.UrlFor(Arg<InputModel>.Is.NotNull));
        }
    }

    [TestFixture]
    public class when_calling_text_box_for
    {
        private IFubuPage<ViewModel> _page;
        private IElementNamingConvention _convention;
        private Expression<Func<ViewModel,object>> _expression;

        [SetUp]
        public void SetUp()
        {
            _page = MockRepository.GenerateMock<IFubuPage<ViewModel>>();
            _convention = MockRepository.GenerateStub<IElementNamingConvention>();
            _expression = (x => x.Property);
            Accessor accessor = _expression.ToAccessor();
            _convention.Stub(c => c.GetName(Arg<Type>.Is.Equal(typeof(ViewModel)), Arg<Accessor>.Is.Equal(accessor))).Return("name");
            _page.Expect(p => p.Get<IElementNamingConvention>()).Return(_convention);
            _page.Expect(p => p.Model).Return(new ViewModel {Property = "some value"});
        }

        [Test]
        public void should_return_text_box_tag()
        {
            _page.TextBoxFor(_expression).ToString().ShouldEqual("<input type=\"text\" name=\"name\" value=\"some value\" />");
            _page.VerifyAllExpectations();
        }

        public class ViewModel { public string Property { get; set; } }
    }

    [TestFixture]
    public class when_calling_input_tag_generating_methods
    {
        private IFubuPage<ViewModel> _page;

        [SetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry(x => x.HtmlConvention<TestHtmlConventions>());
            var container = new Container(x => x.For<IFubuRequest>().Singleton());
            var facility = new StructureMapContainerFacility(container);
            new FubuBootstrapper(facility, registry).Bootstrap(new List<RouteBase>());
            var generator = container.GetInstance<TagGenerator<ViewModel>>();
            
            _page = MockRepository.GenerateMock<IFubuPage<ViewModel>>();
            _page.Expect(p => p.Model).Return(new ViewModel());
            _page.Expect(p => p.Get<TagGenerator<ViewModel>>()).Return(generator);
        }

        [Test]
        public void return_html_tag_on_input_for()
        {
            _page.InputFor(x => x.Name).ToString().ShouldEqual("<input type=\"text\" value=\"\" name=\"Name\" />");
        }

        [Test]
        public void return_html_tag_on_label_for()
        {
            _page.LabelFor(x => x.Name).ToString().ShouldEqual("<span class=\"label\">Name</span>");
        }

        [Test]
        public void return_html_tag_on_display_for()
        {
            _page.DisplayFor(x => x.Name).ToString().ShouldEqual("<span></span>");
        }

        public class ViewModel { public string Name { get; set; } }
    }

    [TestFixture]
    public class when_calling_tag_generating_method_for_arbitrary_type
    {
        private IFubuPage _page;
        private ArbitraryModel _modelFromFubuRequest;

        [SetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry(x => x.HtmlConvention<TestHtmlConventions>());
            var container = new Container(x => x.For<IFubuRequest>().Singleton());
            var facility = new StructureMapContainerFacility(container);
            new FubuBootstrapper(facility, registry).Bootstrap(new List<RouteBase>());
            
            var generator = container.GetInstance<TagGenerator<ArbitraryModel>>();
            _page = MockRepository.GenerateMock<IFubuPage>();
            _page.Stub(p => p.Get<TagGenerator<ArbitraryModel>>()).Return(generator);
            var fubuRequest = MockRepository.GenerateMock<IFubuRequest>();
            _modelFromFubuRequest = new ArbitraryModel{City="Austin"};
            fubuRequest.Stub(x => x.Get<ArbitraryModel>()).Return(_modelFromFubuRequest);
            _page.Stub(p => p.Get<IFubuRequest>()).Return(fubuRequest);
        }

        [Test]
        public void should_populate_the_input_using_a_model_from_fuburequest()
        {
            var tag = _page.InputFor<ArbitraryModel>(x => x.City);
            tag.TagName().ShouldEqual("input");
            tag.Attr("value").ShouldEqual(_modelFromFubuRequest.City);
        }

        [Test]
        public void should_display_the_name_of_the_property_on_the_type()
        {
            var tag = _page.LabelFor<ArbitraryModel>(x => x.City);
            tag.TagName().ShouldEqual("span");
            tag.Text().ShouldEqual("City");
        }

        [Test]
        public void should_display_the_value_of_the_property_from_fuburequest()
        {
            var tag = _page.DisplayFor<ArbitraryModel>(x => x.City);
            tag.TagName().ShouldEqual("span");
            tag.Text().ShouldEqual(_modelFromFubuRequest.City);
        }

        public class ArbitraryModel { public string City { get; set; } }
    }

    [TestFixture]
    public class when_calling_tag_generating_method_for_given_model
    {
        private IFubuPage _page;
        private ArbitraryModel _givenModel;

        [SetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry(x => x.HtmlConvention<TestHtmlConventions>());
            var container = new Container(x => x.For<IFubuRequest>().Singleton());
            var facility = new StructureMapContainerFacility(container);
            new FubuBootstrapper(facility, registry).Bootstrap(new List<RouteBase>());

            var generator = container.GetInstance<TagGenerator<ArbitraryModel>>();
            _page = MockRepository.GenerateMock<IFubuPage>();
            _page.Stub(p => p.Get<TagGenerator<ArbitraryModel>>()).Return(generator);
            _givenModel = new ArbitraryModel { City = "Austin" };
        }

        [Test]
        public void should_populate_the_input_using_the_given_model()
        {
            var tag = _page.InputFor(_givenModel, x => x.City);
            tag.TagName().ShouldEqual("input");
            tag.Attr("value").ShouldEqual(_givenModel.City);
        }

        [Test]
        public void should_display_the_name_of_the_property_on_the_type()
        {
            var tag = _page.LabelFor(_givenModel, x => x.City);
            tag.TagName().ShouldEqual("span");
            tag.Text().ShouldEqual("City");
        }

        [Test]
        public void should_display_the_value_of_the_property_from_the_given_model()
        {
            var tag = _page.DisplayFor(_givenModel, x => x.City);
            tag.TagName().ShouldEqual("span");
            tag.Text().ShouldEqual(_givenModel.City);
        }

        public class ArbitraryModel { public string City { get; set; } }
    }
}