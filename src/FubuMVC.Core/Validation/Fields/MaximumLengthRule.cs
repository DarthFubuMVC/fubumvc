using FubuCore;
using FubuCore.Descriptions;
using FubuCore.Reflection;
using FubuMVC.Core.Localization;

namespace FubuMVC.Core.Validation.Fields
{
    public class MaximumLengthRule : IFieldValidationRule, DescribesItself
    {
        public static readonly string LENGTH = "length";
        private readonly int _length;

        public MaximumLengthRule(int length)
			: this(length, ValidationKeys.MaxLength)
        {
        }

	    public MaximumLengthRule(int length, StringToken token)
	    {
		    _length = length;

		    Token = token;
	    }

	    public StringToken Token { get; set; }

		public ValidationMode Mode { get; set; }

        public int Length
        {
            get { return _length; }
        }

        public void Validate(Accessor accessor, ValidationContext context)
        {
            var rawValue = accessor.GetValue(context.Target);
            if (rawValue != null && rawValue.ToString().Length > Length)
            {
                context.Notification.RegisterMessage(accessor, Token, TemplateValue.For(LENGTH, _length));
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (MaximumLengthRule)) return false;
            return Equals((MaximumLengthRule) obj);
        }

		protected bool Equals(MaximumLengthRule other)
		{
			return _length.Equals(other._length) && Token.Equals(other.Token);
		}

        public override int GetHashCode()
        {
			unchecked
			{
				return (_length * 397) ^ Token.GetHashCode();
			}
        }

	    public void Describe(Description description)
	    {
			description.ShortDescription = "Length: {0}; Message: {1}".ToFormat(_length, Token);
	    }
    }
}