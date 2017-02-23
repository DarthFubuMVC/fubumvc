using FubuCore.Reflection;
using FubuMVC.Core.Validation.Fields;
using FubuMVC.Core.Validation.Web.Remote;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Validation.Web.Remote
{
    
    public class RemoteRuleGraphTester
    {
        private RemoteRuleGraph theGraph = new RemoteRuleGraph();

        [Fact]
        public void registers_rules()
        {
            var a1 = ReflectionHelper.GetAccessor<RuleGraphModel>(x => x.FirstName);
            var a2 = ReflectionHelper.GetAccessor<RuleGraphModel>(x => x.LastName);
            
            var r1 = new RequiredFieldRule();
            var r2 = new MinimumLengthRule(5);

            theGraph.RegisterRule(a1, r1);
            theGraph.RegisterRule(a1, r2);

            theGraph.RegisterRule(a2, r1);

            theGraph.RulesFor(a1).ShouldHaveTheSameElementsAs(RemoteFieldRule.For(a1, r1), RemoteFieldRule.For(a1, r2));
            theGraph.RulesFor(a2).ShouldHaveTheSameElementsAs(RemoteFieldRule.For(a2, r1));
        }

        [Fact]
        public void finds_the_rule_by_the_hash()
        {
            var a1 = ReflectionHelper.GetAccessor<RuleGraphModel>(x => x.FirstName);
            var r1 = new RequiredFieldRule();

            theGraph.RegisterRule(a1, r1);

            var remote = RemoteFieldRule.For(a1, r1);

            theGraph.RuleFor(remote.ToHash()).ShouldBe(remote);
        }

        public class RuleGraphModel
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }
    }
}