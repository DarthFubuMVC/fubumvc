using System;
using System.Linq.Expressions;
using FubuCore.CommandLine;
using NUnit.Framework;
using FubuCore.Reflection;

namespace FubuCore.Testing.CommandLine
{
    [TestFixture]
    public class FlagTester
    {
        private Flag forProp(Expression<Func<FlagTarget, object>> expression)
        {
            return new Flag(expression.ToAccessor().InnerProperty, new ObjectConverter());
        }

        [Test]
        public void to_usage_description_for_a_simple_non_aliased_field()
        {
            forProp(x => x.NameFlag).ToUsageDescription().ShouldEqual("[-name <name>]");
        }

        [Test]
        public void to_usage_description_for_a_simple_aliased_field()
        {
            forProp(x => x.AliasFlag).ToUsageDescription().ShouldEqual("[-a <alias>]");
        }

        [Test]
        public void to_usage_description_for_an_enum_field()
        {
            forProp(x => x.EnumFlag).ToUsageDescription().ShouldEqual("[-enum red|blue|green]");
        }
    }

    public enum FlagEnum
    {
        red, blue, green
    }


    public class FlagTarget
    {
        public string NameFlag { get; set; }

        public FlagEnum EnumFlag { get; set; }

        [FlagAlias("a")]
        public string AliasFlag { get; set;}
    }
}