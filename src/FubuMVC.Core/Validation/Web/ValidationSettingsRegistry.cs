using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Validation.Web
{
	public class ValidationSettingsRegistry
	{
		private readonly IList<ValidationNodeModification> _modifications = new List<ValidationNodeModification>(); 


		public void ForChainsMatching(Func<BehaviorChain, bool> filter, Action<ValidationNode> action)
		{
			addModification(new ValidationNodeModification(filter, action));
		}

		public void ForInputType<T>(Action<ValidationNode> action)
		{
			ForChainsMatching(chain => chain is RoutedChain && chain.InputType() != null, action);
		}
		
		public void ForInputTypesMatching(Func<Type, bool> filter, Action<ValidationNode> action)
		{
			ForChainsMatching(chain => chain.InputType() != null && filter(chain.InputType()), action);
		}

		protected void addModification(ValidationNodeModification modification)
		{
			_modifications.Add(modification);
		}

		internal IEnumerable<ValidationNodeModification> Modifications { get { return _modifications; } }
	}
}