using System;
using System.Linq.Expressions;
using System.ServiceModel.Configuration;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.UI;
using FubuMVC.Core.UI.Configuration;
using FubuMVC.Core.UI.Tags;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using FubuTestingSupport;

using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.UI.Forms
{
    [TestFixture]
    public class when_requesting_a_tag_generator_for_a_strongly_typed_view
    {
        [SetUp]
        public void SetUp()
        {
            var page = MockRepository.GenerateMock<IFubuPage<ViewModel>>();
            page.Stub(x => x.Model).Return(_pageViewModel);
            page.Stub(x => x.ElementPrefix).Return("prefix");
            page.Stub(x => x.Get<ITagGenerator<ViewModel>>()).Return(new TagGenerator<ViewModel>(new TagProfileLibrary(), null, null));

            _generator = page.Tags();
        }

        private readonly ViewModel _pageViewModel = new ViewModel();
        private ITagGenerator<ViewModel> _generator;

        [Test]
        public void the_generator_should_use_the_view_model_of_the_page()
        {
            _generator.Model.ShouldEqual(_pageViewModel);
        }

        [Test]
        public void the_generator_should_use_the_prefix_of_the_page()
        {
            _generator.ElementPrefix.ShouldEqual("prefix");
        }
    }

    [TestFixture]
    public class when_requesting_a_tag_generator_for_an_arbitrary_type
    {
        [SetUp]
        public void SetUp()
        {
            var page = MockRepository.GenerateMock<IFubuPage<ViewModel>>();
            var fubuRequest = MockRepository.GenerateMock<IFubuRequest>();
            fubuRequest.Stub(x => x.Get<InputModel>()).Return(_modelFromFubuRequest);
            page.Stub(x => x.Get<IFubuRequest>()).Return(fubuRequest);
            page.Stub(x => x.ElementPrefix).Return("prefix");
            page.Stub(x => x.Get<ITagGenerator<InputModel>>()).Return(new TagGenerator<InputModel>(new TagProfileLibrary(), null, null));

            _generator = page.Tags<InputModel>();
        }

        private readonly InputModel _modelFromFubuRequest = new InputModel();
        private ITagGenerator<InputModel> _generator;

        [Test]
        public void the_generator_should_use_a_model_from_fubu_request()
        {
            _generator.Model.ShouldEqual(_modelFromFubuRequest);
        }

        [Test]
        public void the_generator_should_use_the_prefix_of_the_page()
        {
            _generator.ElementPrefix.ShouldEqual("prefix");
        }
    }

    [TestFixture]
    public class when_requesting_a_tag_generator_for_a_given_instance
    {
        [SetUp]
        public void SetUp()
        {
            var page = MockRepository.GenerateMock<IFubuPage<ViewModel>>();
            page.Stub(x => x.ElementPrefix).Return("prefix");
            page.Stub(x => x.Get<ITagGenerator<InputModel>>()).Return(new TagGenerator<InputModel>(new TagProfileLibrary(), null, null));

            _generator = page.Tags(_givenInstance);
        }

        private readonly InputModel _givenInstance = new InputModel();
        private ITagGenerator<InputModel> _generator;

        [Test]
        public void the_generator_should_use_the_given_instance_as_its_model()
        {
            _generator.Model.ShouldEqual(_givenInstance);
        }

        [Test]
        public void the_generator_should_use_the_prefix_of_the_page()
        {
            _generator.ElementPrefix.ShouldEqual("prefix");
        }
    }


    [TestFixture]
    public class when_calling_link_variable
    {
        private IFubuPage _page;
        private IUrlRegistry _urls;
        private InputModel _model;

        [SetUp]
        public void SetUp()
        {
            _page = MockRepository.GenerateMock<IFubuPage>();
            _urls = new StubUrlRegistry();

            

            _page.Expect(p => p.Urls).Return(_urls);
            _model = new InputModel();
            //_urls.Stub(u => u.UrlFor(Arg<InputModel>.Is.NotNull)).Return("some url");
        }

        [Test]
        public void should_return_formatted_link_variable()
        {
            _page.LinkVariable("variable", _model).ShouldEqual("var {0} = '{1}';".ToFormat("variable", "url for FubuMVC.Tests.UI.Forms.InputModel"));
            _page.VerifyAllExpectations();
        }

        [Test]
        public void should_return_formatted_link_variable_of_new_model()
        {
            _page.LinkVariable<InputModel>("variable").ShouldEqual("var {0} = '{1}';".ToFormat("variable", "url for FubuMVC.Tests.UI.Forms.InputModel"));
            _page.VerifyAllExpectations();
        }
    }

    [TestFixture]
    public class when_calling_element_name_for
    {
        private IFubuPage<ViewModel> _page;
        private IElementNamingConvention _convention;
        private Expression<Func<ViewModel,object>> _expression;
        private Accessor _accessor;

        [SetUp]
        public void SetUp()
        {
            _page = MockRepository.GenerateStub<IFubuPage<ViewModel>>();
            _convention = MockRepository.GenerateStub<IElementNamingConvention>();
            _expression = (x=>x.Property);
            _accessor = _expression.ToAccessor();
            _convention.Stub(c => c.GetName(Arg<Type>.Is.Equal(typeof (ViewModel)), Arg<Accessor>.Is.Equal(_accessor))).Return("name");
            _page.Stub(p => p.Get<IElementNamingConvention>()).Return(_convention);
        }

        [Test]
        public void should_return_element_name()
        {
            _page.ElementNameFor(_expression).ShouldEqual("name");
            _convention.AssertWasCalled(c => c.GetName(Arg<Type>.Is.Equal(typeof (ViewModel)), Arg<Accessor>.Is.Equal(_accessor)));
        }

        public class ViewModel { public string Property { get; set; } }
    }

    public class ViewModel{}
    public class InputModel{}
    
}