using System;
using FubuCore;
using FubuCore.Descriptions;
using FubuCore.Reflection;
using FubuMVC.Core.Localization;

namespace FubuMVC.Core.Validation.Fields
{
    public class MinValueFieldRule : IFieldValidationRule
    {
        private readonly IComparable _bounds;

        public MinValueFieldRule(IComparable bounds)
			: this(bounds, ValidationKeys.MinValue)
        {
        }

		public MinValueFieldRule(IComparable bounds, StringToken token)
	    {
		    _bounds = bounds;
		    Token = token;
	    }

	    public StringToken Token { get; set; }

		public ValidationMode Mode { get; set; }

        public IComparable Bounds { get { return _bounds; }}

        public void Validate(Accessor accessor, ValidationContext context)
        {
            var value = accessor.GetValue(context.Target);
            if(_bounds.CompareTo(value) > 0)
            {
                context.Notification.RegisterMessage(accessor, Token, TemplateValue.For("bounds", _bounds));
            }
        }

		protected bool Equals(MinValueFieldRule other)
		{
			return _bounds.Equals(other._bounds) && Token.Equals(other.Token);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((MinValueFieldRule)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (_bounds.GetHashCode() * 397) ^ Token.GetHashCode();
			}
		}

		public void Describe(Description description)
		{
			description.ShortDescription = "Bounds: {0}; Message: {1}".ToFormat(_bounds, Token);
		}
    }
}