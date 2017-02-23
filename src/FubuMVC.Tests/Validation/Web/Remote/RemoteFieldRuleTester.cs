using System;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuMVC.Core.Validation.Fields;
using FubuMVC.Core.Validation.Web.Remote;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Validation.Web.Remote
{
    
    public class RemoteFieldRuleTester
    {
        private Accessor accessorFor(Expression<Func<RemoteFieldModel, object>> expression)
        {
            return expression.ToAccessor();
        }

        [Fact]
        public void equality_check()
        {
            var accessor = accessorFor(x => x.Name);

	        var r1 = RemoteFieldRule.For<RequiredFieldRule>(accessor);
			var r2 = RemoteFieldRule.For<RequiredFieldRule>(accessor);

            r1.ShouldBe(r2);
        }

        [Fact]
        public void equality_check_negative_accessor()
        {
            var r1 = RemoteFieldRule.For<RequiredFieldRule>(accessorFor(x => x.Name));
            var r2 = RemoteFieldRule.For<RequiredFieldRule>(accessorFor(x => x.Test));

            r1.ShouldNotBe(r2);
        }

        [Fact]
        public void equality_check_negative_type()
        {
            var accessor = accessorFor(x => x.Name);

			var r1 = RemoteFieldRule.For<RequiredFieldRule>(accessor);
			var r2 = RemoteFieldRule.For<EmailFieldRule>(accessor);

            r1.ShouldNotBe(r2);
        }

        [Fact]
        public void hash_is_repeatable()
        {
			var r1 = RemoteFieldRule.For<RequiredFieldRule>(accessorFor(x => x.Name));
			var r2 = RemoteFieldRule.For<RequiredFieldRule>(accessorFor(x => x.Name));

            r1.ToHash().ShouldBe(r2.ToHash());
        }

        [Fact]
        public void hash_is_unique_by_rule_model_and_accessor()
        {
            var r1 = RemoteFieldRule.For<RequiredFieldRule>(SingleProperty.Build<SomeNamespace.Model>(e => e.Property));
			var r2 = RemoteFieldRule.For<RequiredFieldRule>(SingleProperty.Build<OtherNamespace.Model>(e => e.Property));

            r1.ToHash().ShouldNotBe(r2.ToHash());
        }

        public class RemoteFieldModel
        {
            public string Name { get; set; }
            public string Test { get; set; }
        }
    }

    namespace SomeNamespace
    {
        public class Model
        {
            public string Property { get; set; }
        }
    }

    namespace OtherNamespace
    {
        public class Model
        {
            public string Property { get; set; }
        }
    }

}
