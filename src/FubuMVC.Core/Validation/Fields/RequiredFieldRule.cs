using FubuCore.Descriptions;
using FubuCore.Reflection;
using FubuMVC.Core.Localization;

namespace FubuMVC.Core.Validation.Fields
{
    public class RequiredFieldRule : IFieldValidationRule, DescribesItself
    {
	    public RequiredFieldRule()
			: this(ValidationKeys.Required)
	    {
	    }

	    public RequiredFieldRule(StringToken token)
	    {
		    Token = token;
	    }

		public StringToken Token { get; set; }

		public ValidationMode Mode { get; set; }

        public void Validate(Accessor accessor, ValidationContext context)
        {
            var rawValue = accessor.GetValue(context.Target);

            if (rawValue == null || string.Empty.Equals(rawValue))
            {
                context.Notification.RegisterMessage(accessor, Token);
            }
        }

        public bool Equals(RequiredFieldRule other)
        {
            return Token.Equals(other.Token);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (RequiredFieldRule)) return false;
            return Equals((RequiredFieldRule) obj);
        }

        public override int GetHashCode()
        {
            return Token.GetHashCode();
        }

	    public void Describe(Description description)
	    {
		    description.ShortDescription = Token.ToString();
	    }
    }
}