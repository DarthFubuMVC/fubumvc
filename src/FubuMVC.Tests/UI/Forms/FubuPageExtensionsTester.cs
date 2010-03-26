using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using FubuMVC.Core.View.WebForms;
using FubuMVC.UI;
using FubuMVC.UI.Configuration;
using FubuMVC.UI.Tags;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.UI.Forms
{
    [TestFixture]
    public class when_calling_tags
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _page = MockRepository.GenerateMock<IFubuPage<ViewModel>>();
            var namingConvention = MockRepository.GenerateStub<IElementNamingConvention>();
            var serviceLocator = MockRepository.GenerateStub<IServiceLocator>();
            _tags = new TagGenerator<ViewModel>(new TagProfileLibrary(), namingConvention, 
                serviceLocator, new Stringifier());

            _page.Stub(x => x.Model).Return(_model);
            _page.Stub(x => x.ElementPrefix).Return("prefix");
        }

        #endregion

        private IFubuPage<ViewModel> _page;
        private TagGenerator<ViewModel> _tags;
        private readonly ViewModel _model = new ViewModel();

        [Test]
        public void should_return_tag_generator()
        {
            _page.Stub(x => x.Get<TagGenerator<ViewModel>>()).Return(_tags);

            ITagGenerator<ViewModel> generator = _page.Tags();
            generator.Model.ShouldEqual(_model);
            generator.ElementPrefix.ShouldEqual("prefix");
        }
    }

    [TestFixture]
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
                serviceLocator, new Stringifier());
            
            _viewTypeRegistry = MockRepository.GenerateStub<IPartialViewTypeRegistry>();
            serviceLocator.Stub(s => s.GetInstance<IPartialViewTypeRegistry>()).Return(_viewTypeRegistry);
            
            _model = new InputModel{Partials=new List<PartialModel>{new PartialModel()}};
            _page.Expect(p => p.Get<TagGenerator<InputModel>>()).Return(_tags);
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