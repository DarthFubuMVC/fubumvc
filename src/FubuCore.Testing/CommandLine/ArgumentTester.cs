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
    }

    public enum TargetEnum
    {
        red,blue,green
    }

    public class ArgumentTarget
    {
        public TargetEnum Enum{ get; set;}
        public string Name { get; set; }

        [System.ComponentModel.Description("age of target")]
        public int Age { get; set; }

    }
}