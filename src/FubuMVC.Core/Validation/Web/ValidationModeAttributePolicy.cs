using System;
using FubuCore;
using FubuCore.Reflection;

namespace FubuMVC.Core.Validation.Web
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
	public class ValidationModeAttribute : Attribute
	{
		private readonly ValidationMode _mode;

		protected ValidationModeAttribute(ValidationMode mode)
		{
			_mode = mode;
		}

		public ValidationMode Mode
		{
			get { return _mode; }
		}
	}

	public class LiveValidationAttribute : ValidationModeAttribute
	{
		public LiveValidationAttribute()
			: base(ValidationMode.Live)
		{
		}
	}

	public class TriggeredValidationAttribute : ValidationModeAttribute
	{
		public TriggeredValidationAttribute()
			: base(ValidationMode.Triggered)
		{
		}
	}

	public class ValidationModeAttributePolicy : IValidationModePolicy
	{
		public bool Matches(IServiceLocator services, Accessor accessor)
		{
			return accessor.HasAttribute<ValidationModeAttribute>()
				|| accessor.OwnerType.HasAttribute<ValidationModeAttribute>();
		}

		public ValidationMode DetermineMode(IServiceLocator services, Accessor accessor)
		{
			if (accessor.OwnerType.HasAttribute<ValidationModeAttribute>())
			{
				return accessor.OwnerType.GetAttribute<ValidationModeAttribute>().Mode;
			}

			return accessor.GetAttribute<ValidationModeAttribute>().Mode;
		}
	}
}