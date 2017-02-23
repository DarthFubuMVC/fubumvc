using FubuCore.Binding.InMemory;
using FubuCore.Reflection;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Tests.Urls;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Http
{
    
    public class CurrentRequestRelativeUrlPropertyBinderTester
    {
        private CurrentRequestRelativeUrlPropertyBinder binder = new CurrentRequestRelativeUrlPropertyBinder();

        [Fact]
        public void should_match_on_correct_property_name()
        {
            binder.Matches(ReflectionHelper.GetProperty<RelativeUrlModel>(f => f.RelativeUrl)).ShouldBeTrue();
        }

        [Fact]
        public void should_not_match_on_other_name()
        {
            binder.Matches(ReflectionHelper.GetProperty<RelativeUrlModel>(f => f.Blah)).ShouldBeFalse();
        }

        [Fact]
        public void should_populate_relativeurl_from_current_http_request()
        {

            

            var stubCurrentHttpRequest = OwinHttpRequest.ForTesting().FullUrl("http://server/foo");
                
                
                
            var model =
                BindingScenario<RelativeUrlModel>.For(x =>
                                                          {
                                                              x.BindPropertyWith<CurrentRequestRelativeUrlPropertyBinder>(f => f.RelativeUrl);
                                                              x.Service<IHttpRequest>(stubCurrentHttpRequest);
                                                          })
                                                          .Model;
            model.RelativeUrl.ShouldBe(stubCurrentHttpRequest.RelativeUrl());
        }

        public class RelativeUrlModel
        {
            public string RelativeUrl { get; set; }
            public string Blah { get; set; }
        }
    }
}