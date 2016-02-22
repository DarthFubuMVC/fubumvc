using FubuCore;
using FubuCore.Descriptions;
using FubuCore.Reflection;
using FubuMVC.Core.Localization;

namespace FubuMVC.Core.Validation.Fields
{
    public class MinimumLengthRule : IFieldValidationRule, DescribesItself
    {
	    public static readonly string LENGTH = "length";
        private readonly int _length;

        public MinimumLengthRule(int length)
			: this(length, ValidationKeys.MinLength)
        {
        }

	    public MinimumLengthRule(int length, StringToken token)
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
            var value = context.GetFieldValue<string>(accessor);
            if (value != null && value.Length < Length)
            {
                context.Notification.RegisterMessage(accessor, Token, TemplateValue.For(LENGTH, _length));
            }
        }

		protected bool Equals(MinimumLengthRule other)
		{
			return _length == other._length && Token.Equals(other.Token);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((MinimumLengthRule)obj);
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