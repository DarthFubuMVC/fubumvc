using System;
using System.Linq.Expressions;
using System.Web;
using FubuCore.Binding;
using FubuCore.Reflection;
using FubuMVC.Core.Runtime;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Runtime
{
    [TestFixture]
    public class AspNetPassthroughConverterTester
    {
        private bool matches(Expression<Func<FakeModel, object>> expression)
        {
            var property = ReflectionHelper.GetProperty(expression);
            return new AspNetPassthroughConverter().Matches(property);
        }


        [Test]
        public void matches_positive()
        {
            matches(x => x.File).ShouldBeTrue();
            matches(x => x.Cookie).ShouldBeTrue();
            matches(x => x.Wrapper).ShouldBeTrue();
        }

        [Test]
        public void matches_negative()
        {
            matches(x => x.Name).ShouldBeFalse();
        }

        [Test]
        public void builds_a_pass_thru_converter()
        {
            var property = ReflectionHelper.GetProperty<FakeModel>(x => x.File);
            new AspNetPassthroughConverter().Build(null, property)
                .ShouldBeOfType<PassthroughConverter<HttpPostedFileBase>>();
        }

        public class FakeModel
        {
            public HttpPostedFileBase File { get; set; }
            public HttpCookie Cookie { get; set; }
            public HttpFileCollectionWrapper Wrapper { get; set; }

            public string Name { get; set; }
        }
    }
}