using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Security.Authorization;

namespace FubuMVC.Core.Validation.Web.Remote
{
    public class ValidateField
    {
        public string Hash { get; set; }
        public string Value { get; set; }
    }

    public class ValidateFieldEndpoint
    {
        private readonly RemoteRuleGraph _graph;
        private readonly IRuleRunner _rules;
        private readonly IAjaxContinuationResolver _continuation;

        public ValidateFieldEndpoint(RemoteRuleGraph graph, IRuleRunner rules, IAjaxContinuationResolver continuation)
        {
            _graph = graph;
            _rules = rules;
            _continuation = continuation;
        }

        [UrlPattern("_validation/remote")]
        public AjaxContinuation Validate(ValidateField field)
        {
            var rule = _graph.RuleFor(field.Hash);
            var notification = _rules.Run(rule, field.Value);

            return _continuation.Resolve(notification);
        }
    }
    
    [Description("Remote Validation Rules")]
    public class RemoteRulesSource : IActionSource
    {
        Task<ActionCall[]> IActionSource.FindActions(Assembly applicationAssembly)
        {
            var actionCall = ActionCall.For<ValidateFieldEndpoint>(x => x.Validate(null));
            return Task.FromResult(new[] {actionCall});
        }

    }
}