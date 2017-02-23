using System;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using Shouldly;
using Xunit;
using Rhino.Mocks;

namespace FubuMVC.Tests.UI
{


    
    public class when_calling_link_variable
    {
        public when_calling_link_variable()
        {
            _page = MockRepository.GenerateMock<IFubuPage>();
            _urls = new StubUrlRegistry();


            _page.Expect(p => p.Urls).Return(_urls);
            _model = new InputModel();
            //_urls.Stub(u => u.UrlFor(Arg<InputModel>.Is.NotNull)).Return("some url");
        }


        private IFubuPage _page;
        private IUrlRegistry _urls;
        private InputModel _model;

        [Fact]
        public void should_return_formatted_link_variable()
        {
            _page.LinkVariable("variable", _model).ShouldBe("var {0} = '{1}';".ToFormat("variable",
                                                                                           "url for FubuMVC.Tests.UI.InputModel, category GET"));
            _page.VerifyAllExpectations();
        }

        [Fact]
        public void should_return_formatted_link_variable_of_new_model()
        {
            _page.LinkVariable<InputModel>("variable").ShouldBe("var {0} = '{1}';".ToFormat("variable",
                                                                                               "url for FubuMVC.Tests.UI.InputModel, category GET"));
            _page.VerifyAllExpectations();
        }
    }

    public class ViewModel
    {
    }

    public class InputModel
    {
    }
}