using System;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.UI;
using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.UI
{
    [TestFixture]
    public class when_calling_text_box_for_and_model_has_null_property : when_calling_text_box_for
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            BaseSetUp();
            _page.Expect(p => p.Model).Return(new ViewModel());
        }

        #endregion

        [Test]
        public void should_return_text_box_tag_with_blank_value()
        {
            _page.TextBoxFor(_expression).ToString().ShouldEqual("<input type=\"text\" name=\"name\" value=\"\" />");
            _page.VerifyAllExpectations();
        }
    }

    public abstract class when_calling_text_box_for
    {
        protected IElementNamingConvention _convention;
        protected Expression<Func<ViewModel, object>> _expression;
        protected IFubuPage<ViewModel> _page;

        public void BaseSetUp()
        {
            _page = MockRepository.GenerateMock<IFubuPage<ViewModel>>();
            _convention = MockRepository.GenerateStub<IElementNamingConvention>();
            _expression = (x => x.Property);
            Accessor accessor = _expression.ToAccessor();
            _convention.Stub(c => c.GetName(Arg<Type>.Is.Equal(typeof (ViewModel)), Arg<Accessor>.Is.Equal(accessor))).
                Return("name");
            _page.Expect(p => p.Get<IElementNamingConvention>()).Return(_convention);
        }

        #region Nested type: ViewModel

        public class ViewModel
        {
            public string Property { get; set; }
        }

        #endregion
    }

    [TestFixture]
    public class when_calling_link_variable
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _page = MockRepository.GenerateMock<IFubuPage>();
            _urls = new StubUrlRegistry();


            _page.Expect(p => p.Urls).Return(_urls);
            _model = new InputModel();
            //_urls.Stub(u => u.UrlFor(Arg<InputModel>.Is.NotNull)).Return("some url");
        }

        #endregion

        private IFubuPage _page;
        private IUrlRegistry _urls;
        private InputModel _model;

        [Test]
        public void should_return_formatted_link_variable()
        {
            _page.LinkVariable("variable", _model).ShouldEqual("var {0} = '{1}';".ToFormat("variable",
                                                                                           "url for FubuMVC.Tests.UI.InputModel, category GET"));
            _page.VerifyAllExpectations();
        }

        [Test]
        public void should_return_formatted_link_variable_of_new_model()
        {
            _page.LinkVariable<InputModel>("variable").ShouldEqual("var {0} = '{1}';".ToFormat("variable",
                                                                                               "url for FubuMVC.Tests.UI.InputModel, category GET"));
            _page.VerifyAllExpectations();
        }
    }

    [TestFixture]
    public class when_calling_element_name_for
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _page = MockRepository.GenerateStub<IFubuPage<ViewModel>>();
            _convention = MockRepository.GenerateStub<IElementNamingConvention>();
            _expression = (x => x.Property);
            _accessor = _expression.ToAccessor();
            _convention.Stub(c => c.GetName(Arg<Type>.Is.Equal(typeof (ViewModel)), Arg<Accessor>.Is.Equal(_accessor))).
                Return("name");
            _page.Stub(p => p.Get<IElementNamingConvention>()).Return(_convention);
        }

        #endregion

        private IFubuPage<ViewModel> _page;
        private IElementNamingConvention _convention;
        private Expression<Func<ViewModel, object>> _expression;
        private Accessor _accessor;

        public class ViewModel
        {
            public string Property { get; set; }
        }

        [Test]
        public void should_return_element_name()
        {
            _page.ElementNameFor(_expression).ShouldEqual("name");
            _convention.AssertWasCalled(
                c => c.GetName(Arg<Type>.Is.Equal(typeof (ViewModel)), Arg<Accessor>.Is.Equal(_accessor)));
        }
    }

    public class ViewModel
    {
    }

    public class InputModel
    {
    }
}