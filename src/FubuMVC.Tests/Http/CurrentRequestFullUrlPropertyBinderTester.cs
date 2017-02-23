﻿using FubuCore.Binding.InMemory;
using FubuCore.Reflection;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Owin;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Http
{
    
    public class CurrentRequestFullUrlPropertyBinderTester
    {
        private CurrentRequestFullUrlPropertyBinder binder = new CurrentRequestFullUrlPropertyBinder();


        [Fact]
        public void should_match_on_correct_property_name()
        {
            binder.Matches(ReflectionHelper.GetProperty<FullUrlModel>(f => f.FullUrl)).ShouldBeTrue();
        }

        [Fact]
        public void should_not_match_on_other_name()
        {
            binder.Matches(ReflectionHelper.GetProperty<FullUrlModel>(f => f.Blah)).ShouldBeFalse();
        }

        [Fact]
        public void should_populate_fullurl_from_current_http_request()
        {
            var stubCurrentHttpRequest = OwinHttpRequest.ForTesting();
            var model =
                BindingScenario<FullUrlModel>.For(x => {
                    x.BindPropertyWith<CurrentRequestFullUrlPropertyBinder>(f => f.FullUrl);
                    x.Service<IHttpRequest>(stubCurrentHttpRequest);
                })
                    .Model;
            model.FullUrl.ShouldBe(stubCurrentHttpRequest.FullUrl());
        }

        public class FullUrlModel
        {
            public string FullUrl { get; set; }
            public string Blah { get; set; }
        }
    }
}