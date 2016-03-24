using FubuCore;
using FubuCore.Reflection;

namespace FubuMVC.Core.Validation.Web
{
	public interface IValidationModePolicy
	{
		bool Matches(IServiceLocator services, Accessor accessor);
		ValidationMode DetermineMode(IServiceLocator services, Accessor accessor);
	}
}