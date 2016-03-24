using FubuCore;
using FubuCore.Reflection;

namespace FubuMVC.Core.Validation.Web
{
	public class AccessorRulesValidationModePolicy : IValidationModePolicy
	{
		public bool Matches(IServiceLocator services, Accessor accessor)
		{
			return DetermineMode(services, accessor) != null;
		}

		public ValidationMode DetermineMode(IServiceLocator services, Accessor accessor)
		{
			var rules = services.GetInstance<AccessorRules>();
			return rules.FirstRule<ValidationMode>(accessor);
		}
	}
}