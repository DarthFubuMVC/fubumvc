using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore.CommandLine;
using FubuCore.Reflection;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.CommandLine
{
    [TestFixture]
    public class EnumerableArgumentTester
    {
        private EnumerableArgument argFor(Expression<Func<EnumerableArgumentTarget, object>> property)
        {
            return new EnumerableArgument(property.ToAccessor().InnerProperty, new ObjectConverter());
        }

        [Test]
        public void description_is_just_the_property_name_if_no_description_attribute()
        {
            argFor(x => x.Names).Description.ShouldEqual("Names");
        }

        [Test]
        public void description_comes_from_the_attribute_if_it_exists()
        {
            argFor(x => x.Ages).Description.ShouldEqual("ages of target");
        }

        [Test]
        public void to_usage_description_with_a_simple_string_or_number_type()
        {
            argFor(x => x.Names).ToUsageDescription().ShouldEqual("<names1 names2 names3 ...>");
            argFor(x => x.Ages).ToUsageDescription().ShouldEqual("<ages1 ages2 ages3 ...>");
        }

    }

    
    public class EnumerableArgumentTarget
    {
        [RequiredUsage("a", "b", "c")]
        public IEnumerable<string> Names { get; set; }

        [RequiredUsage("c")]
        public IEnumerable<TargetEnum> Enums { get; set; }

        [System.ComponentModel.Description("ages of target")]
        public IEnumerable<int> Ages { get; set; }

    }
}