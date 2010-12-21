using System;
using System.Linq.Expressions;
using FubuCore.CommandLine;
using FubuCore.Reflection;
using NUnit.Framework;

namespace FubuCore.Testing.CommandLine
{
    [TestFixture]
    public class ArgumentTester
    {
        private Argument argFor(Expression<Func<ArgumentTarget, object>> property)
        {
            return new Argument(property.ToAccessor().InnerProperty, new ObjectConverter());
        }

        [Test]
        public void description_is_just_the_property_name_if_no_description_attribute()
        {
            argFor(x => x.Name).Description.ShouldEqual("Name");
        }

        [Test]
        public void description_comes_from_the_attribute_if_it_exists()
        {
            argFor(x => x.Age).Description.ShouldEqual("age of target");
        }

        [Test]
        public void to_usage_description_with_a_simple_string_or_number_type()
        {
            argFor(x => x.Name).ToUsageDescription().ShouldEqual("<name>");
            argFor(x => x.Age).ToUsageDescription().ShouldEqual("<age>");
        }

        [Test]
        public void to_usage_description_with_an_enumeration()
        {
            argFor(x => x.Enum).ToUsageDescription().ShouldEqual("red|blue|green");
        }

        [Test]
        public void required_forUsage_respects_the_required_usage_attribute()
        {
            argFor(x => x.Enum).RequiredForUsage("a").ShouldBeFalse();
            argFor(x => x.Enum).RequiredForUsage("b").ShouldBeFalse();
            argFor(x => x.Enum).RequiredForUsage("c").ShouldBeTrue();

            argFor(x => x.Name).RequiredForUsage("a").ShouldBeTrue();
            argFor(x => x.Name).RequiredForUsage("b").ShouldBeTrue();
            argFor(x => x.Name).RequiredForUsage("c").ShouldBeTrue();
        }

        [Test]
        public void arg_is_never_optional()
        {
            argFor(x => x.Enum).OptionalForUsage("a").ShouldBeFalse();
            argFor(x => x.Enum).OptionalForUsage("b").ShouldBeFalse();
            argFor(x => x.Enum).OptionalForUsage("c").ShouldBeFalse();
            argFor(x => x.Enum).OptionalForUsage("d").ShouldBeFalse();
        }
    }

    public enum TargetEnum
    {
        red,blue,green
    }

    public class ArgumentTarget
    {
        [RequiredUsage("a", "b", "c")]
        public string Name { get; set; }

        [RequiredUsage( "c")]
        public TargetEnum Enum{ get; set;}

        [System.ComponentModel.Description("age of target")]
        public int Age { get; set; }

    }
}