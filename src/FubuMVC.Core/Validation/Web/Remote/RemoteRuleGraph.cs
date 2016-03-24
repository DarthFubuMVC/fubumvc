using System.Collections.Generic;
using System.Linq;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuMVC.Core.Validation.Fields;

namespace FubuMVC.Core.Validation.Web.Remote
{
    public class RemoteRuleGraph
    {
        private readonly Cache<Accessor, IList<RemoteFieldRule>> _rules;
        private readonly Cache<string, RemoteFieldRule> _lookup;

        public RemoteRuleGraph()
        {
            _rules = new Cache<Accessor, IList<RemoteFieldRule>>(x => new List<RemoteFieldRule>());
            _lookup = new Cache<string, RemoteFieldRule>(hash =>
	        {
		        return _rules.SelectMany(r => r).SingleOrDefault(r => r.Matches(hash));
	        });
        }

        public RemoteFieldRule RegisterRule(Accessor accessor, IFieldValidationRule rule)
        {
            var remote = RemoteFieldRule.For(accessor, rule);
            _rules[accessor].Fill(remote);

            return remote;
        }

        public IEnumerable<RemoteFieldRule> RulesFor(Accessor accessor)
        {
            return _rules[accessor];
        }

        public RemoteFieldRule RuleFor(string hash)
        {
            return _lookup[hash];
        }
    }
}