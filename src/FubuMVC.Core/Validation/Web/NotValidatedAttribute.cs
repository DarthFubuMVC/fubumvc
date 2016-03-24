using System;

namespace FubuMVC.Core.Validation.Web
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class NotValidatedAttribute : Attribute
	{
	}
}