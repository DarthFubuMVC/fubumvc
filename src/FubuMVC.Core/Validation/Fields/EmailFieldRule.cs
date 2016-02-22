using System.Text.RegularExpressions;
using FubuCore;
using FubuCore.Descriptions;
using FubuCore.Reflection;
using FubuMVC.Core.Localization;

namespace FubuMVC.Core.Validation.Fields
{
    public class EmailFieldRule : IFieldValidationRule, DescribesItself
    {
	    private static readonly Regex EmailExpression = new Regex(@"^(?:[\w\!\#\$\%\&\'\*\+\-\/\=\?\^\`\{\|\}\~]+\.)*[\w\!\#\$\%\&\'\*\+\-\/\=\?\^\`\{\|\}\~]+@(?:(?:(?:[a-zA-Z0-9](?:[a-zA-Z0-9\-](?!\.)){0,61}[a-zA-Z0-9]?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9\-](?!$)){0,61}[a-zA-Z0-9]?)|(?:\[(?:(?:[01]?\d{1,2}|2[0-4]\d|25[0-5])\.){3}(?:[01]?\d{1,2}|2[0-4]\d|25[0-5])\]))$", RegexOptions.Compiled);

	    public EmailFieldRule()
			: this(ValidationKeys.Email)
	    {
	    }

	    public EmailFieldRule(StringToken token)
	    {
		    Token = token;
	    }

	    public StringToken Token { get; set; }

		public ValidationMode Mode { get; set; }

        public void Validate(Accessor accessor, ValidationContext context)
        {
            var email = context.GetFieldValue<string>(accessor);

	         if(email.IsEmpty()) return;

            if(!EmailExpression.IsMatch(email))
            {
                context.Notification.RegisterMessage(accessor, Token);
            }
        }

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((EmailFieldRule)obj);
		}

		protected bool Equals(EmailFieldRule other)
		{
			return Token.Equals(other.Token);
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