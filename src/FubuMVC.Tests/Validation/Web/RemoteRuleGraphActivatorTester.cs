using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore.Reflection;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuMVC.Core.Localization;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Validation;
using FubuMVC.Core.Validation.Fields;
using FubuMVC.Core.Validation.Web;
using FubuMVC.Core.Validation.Web.Remote;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Validation.Web
{
    
    public class RemoteRuleGraphActivatorTester
    {
        private ValidationGraph theValidationGraph;
        private RemoteRuleGraph theRuleGraph;
        private BehaviorGraph theBehaviorGraph;
        private RemoteRuleQuery theQuery;
        private RemoteRuleGraphActivator theActivator;

        public RemoteRuleGraphActivatorTester()
        {
            theBehaviorGraph = BehaviorGraph.BuildFrom(r =>
            {
                r.Actions.IncludeType<RemoteRuleGraphEndpoint>();
                r.Features.Validation.Enable(true);
            });

            theValidationGraph = ValidationGraph.BasicGraph();
            theRuleGraph = new RemoteRuleGraph();
            theQuery = RemoteRuleQuery.Basic();

            theActivator = new RemoteRuleGraphActivator(theValidationGraph, theRuleGraph, theBehaviorGraph, theQuery, new TypeDescriptorCache());

            theActivator.Activate(new ActivationLog(), new PerfTimer());
        }

        [Fact]
        public void fills_the_rules_in_the_rule_graph()
        {
            var rule = theRuleGraph.RulesFor(ReflectionHelper.GetAccessor<ActivatorTargetWithRemotes>(x => x.Username)).Single();
            rule.Type.ShouldBe(typeof(UniqueUsernameRule));

            theRuleGraph.RulesFor(ReflectionHelper.GetAccessor<ActivatorTargetWithRemotes>(x => x.Name)).ShouldHaveCount(0);
            theRuleGraph.RulesFor(ReflectionHelper.GetAccessor<ActivatorTargetNoRemotes>(x => x.Name)).ShouldHaveCount(0);
        }

        public class RemoteRuleGraphEndpoint
        {
            public AjaxContinuation get_remotes(ActivatorTargetWithRemotes request)
            {
                throw new NotImplementedException();
            }

            public AjaxContinuation get_no_remotes(ActivatorTargetNoRemotes request)
            {
                throw new NotImplementedException();
            }
        }

        public class ActivatorTargetWithRemotes
        {
            [UniqueUsername]
            public string Username { get; set; }

            public string Name { get; set; }
        }

        public class ActivatorTargetNoRemotes
        {
            [Required]
            public string Name { get; set; }
        }


    }

    public class UniqueUsernameRule : IRemoteFieldValidationRule
    {
	    public StringToken Token { get; set; }

		public ValidationMode Mode { get; set; }

	    public void Validate(Accessor accessor, ValidationContext context)
        {
            throw new System.NotImplementedException();
        }
    }

    public class UniqueUsernameAttribute : FieldValidationAttribute
    {
        public override IEnumerable<IFieldValidationRule> RulesFor(PropertyInfo property)
        {
            yield return new UniqueUsernameRule();
        }
    }
}