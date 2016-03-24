using System;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Validation.Web
{
	public class ValidationNodeModification
	{
		private readonly Func<BehaviorChain, bool> _filter;
		private readonly Action<ValidationNode> _action;

		public ValidationNodeModification(Func<BehaviorChain, bool> filter, Action<ValidationNode> action)
		{
			_filter = filter;
			_action = action;
		}

		public bool Matches(BehaviorChain chain)
		{
			return _filter(chain);
		}

		public void Modify(BehaviorChain chain)
		{
			_action(chain.ValidationNode());
		}
	}
}