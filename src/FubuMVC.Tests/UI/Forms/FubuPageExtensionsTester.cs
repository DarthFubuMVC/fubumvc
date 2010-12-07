using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.UI;
using FubuMVC.Core.UI.Configuration;
using FubuMVC.Core.UI.Tags;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using FubuMVC.Core.View.WebForms;
using Microsoft.Practices.ServiceLocation;
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

    [TestFixture, Ignore("Temporarily ignoring these until UI is merged into FubuMVC.Core")]
    public class when_calling_partial
    {
        private IFubuPage _page;
        private IPartialFactory _partialFactory;
        private IActionBehavior _behavior;
        private IFubuRequest _request;

        [SetUp]
        public void SetUp()
        {
            _page = MockRepository.GenerateStub<IFubuPage>();
            _partialFactory = MockRepository.GenerateStub<IPartialFactory>();
            _behavior = MockRepository.GenerateStub<IActionBehavior>();
            
            _partialFactory.Stub(f => f.BuildPartial(typeof (InputModel))).Return(_behavior);
            _page.Stub(p => p.Get<IPartialFactory>()).Return(_partialFactory);
            

            _request = MockRepository.GenerateStub<IFubuRequest>();
            _page.Stub(p => p.Get<IFubuRequest>()).Return(_request);
        }

        [Test]
        public void should_build_partial()
        {
            _page.Partial<InputModel>();

            _partialFactory.AssertWasCalled(f=>f.BuildPartial(typeof(InputModel)));
            _page.AssertWasCalled(p=>p.Get<IPartialFactory>());
            _behavior.AssertWasCalled(b=>b.InvokePartial());
        }

        [Test]
        public void should_build_partial_and_set_model()
        {
            var model = new InputModel();
            _page.Partial(model);

            _request.AssertWasCalled(r=>r.Set(model));
            _partialFactory.AssertWasCalled(f => f.BuildPartial(typeof(InputModel)));
            _page.AssertWasCalled(p => p.Get<IPartialFactory>());
            _behavior.AssertWasCalled(b => b.InvokePartial());
        }
    }

    [TestFixture]
    public class when_calling_partial_for_each
    {
        private IFubuPage<InputModel> _page;
        private InputModel _model;
        private IPartialRenderer _renderer;
        private IPartialViewTypeRegistry _viewTypeRegistry;
        private TagGenerator<InputModel> _tags;

        [SetUp]
        public void SetUp()
        {
            _page = MockRepository.GenerateMock<IFubuPage<InputModel>>();
            _renderer = MockRepository.GenerateStub<IPartialRenderer>();
            var serviceLocator = MockRepository.GenerateStub<IServiceLocator>();
            var namingConvention = MockRepository.GenerateStub<IElementNamingConvention>();
            _tags = new TagGenerator<InputModel>(new TagProfileLibrary(), namingConvention,
                serviceLocator);
            
            _viewTypeRegistry = MockRepository.GenerateStub<IPartialViewTypeRegistry>();
            serviceLocator.Stub(s => s.GetInstance<IPartialViewTypeRegistry>()).Return(_viewTypeRegistry);

            var inMemoryFubuRequest = new InMemoryFubuRequest();
            inMemoryFubuRequest.Set(new InputModel());

            _page.Stub(s => s.Get<IFubuRequest>()).Return(inMemoryFubuRequest);
            
            _model = new InputModel{Partials=new List<PartialModel>{new PartialModel()}};
            _page.Expect(p => p.Get<ITagGenerator<InputModel>>()).Return(_tags);
            _page.Expect(p => p.Model).Return(_model);
            _page.Expect(p => p.Get<IPartialRenderer>()).Return(_renderer);
            _page.Expect(p => p.ServiceLocator).Return(serviceLocator);
        }

        [Test]
        public void should_return_expression_without_calling_using()
        {
            _viewTypeRegistry.Stub(r => r.HasPartialViewTypeFor<PartialModel>()).Return(false);

            _page.PartialForEach(x=>x.Partials);
            
            _page.VerifyAllExpectations();
            _viewTypeRegistry.AssertWasCalled(r => r.HasPartialViewTypeFor<PartialModel>());
            _viewTypeRegistry.AssertWasNotCalled(r => r.GetPartialViewTypeFor<PartialModel>());
            _renderer.AssertWasNotCalled(r => r.CreateControl(typeof(PartialView)));
        }

        [Test]
        public void should_return_expression_after_calling_using()
        {
            _viewTypeRegistry.Stub(r => r.HasPartialViewTypeFor<PartialModel>()).Return(true);
            _viewTypeRegistry.Stub(r => r.GetPartialViewTypeFor<PartialModel>()).Return(typeof (PartialView));

            _page.PartialForEach(x => x.Partials);

            _page.VerifyAllExpectations();
            _viewTypeRegistry.AssertWasCalled(r => r.HasPartialViewTypeFor<PartialModel>());
            _viewTypeRegistry.AssertWasCalled(r => r.GetPartialViewTypeFor<PartialModel>());
            _renderer.AssertWasCalled(r => r.CreateControl(typeof(PartialView)));
        }

        public class PartialModel { }
        public class InputModel { public IList<PartialModel> Partials { get; set; } }
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
            _urls = MockRepository.GenerateStub<IUrlRegistry>();
            _page.Expect(p => p.Urls).Return(_urls);
            _model = new InputModel();
            _urls.Stub(u => u.UrlFor(Arg<InputModel>.Is.NotNull)).Return("some url");
        }

        [Test]
        public void should_return_formatted_link_variable()
        {
            _page.LinkVariable("variable", _model).ShouldEqual("var {0} = '{1}';".ToFormat("variable", "some url"));
            _urls.AssertWasCalled(u=>u.UrlFor(_model));
            _page.VerifyAllExpectations();
        }

        [Test]
        public void should_return_formatted_link_variable_of_new_model()
        {
            _page.LinkVariable<InputModel>("variable").ShouldEqual("var {0} = '{1}';".ToFormat("variable", "some url"));
            _urls.AssertWasNotCalled(u => u.UrlFor(_model));
            _page.VerifyAllExpectations();
            _urls.AssertWasCalled(u => u.UrlFor(Arg<InputModel>.Is.NotNull));
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
    public class PartialView : FubuPage {}
}