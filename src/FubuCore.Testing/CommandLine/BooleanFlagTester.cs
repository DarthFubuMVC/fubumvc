using System;
using System.Linq.Expressions;
using FubuCore.CommandLine;
using NUnit.Framework;
using FubuCore.Reflection;

namespace FubuCore.Testing.CommandLine
{
    [TestFixture]
    public class BooleanFlagTester
    {

        private BooleanFlag getFlag(Expression<Func<BooleanFlagTarget, object>> expression)
        {
            return new BooleanFlag(expression.ToAccessor().InnerProperty);
        }


        [Test]
        public void get_usage_description_with_an_alias()
        {
            getFlag(x => x.AliasedFlag).ToUsageDescription().ShouldEqual("[-f]");
        }

        [Test]
        public void get_usage_description_without_an_alias()
        {
            getFlag(x => x.NormalFlag).ToUsageDescription().ShouldEqual("[-normal]");
        }
    }

    public class BooleanFlagTarget
    {
        [FlagAlias("f")]
        public bool AliasedFlag { get; set; }
        public bool NormalFlag { get; set; }
    }
}