using FubuCore;
using FubuCore.Descriptions;
using FubuCore.Reflection;
using FubuMVC.Core.Localization;

namespace FubuMVC.Core.Validation.Fields
{
    public class CollectionLengthRule : IFieldValidationRule, DescribesItself
    {
	    private readonly int _length;
        public static readonly string LENGTH = "min";

        public CollectionLengthRule(int length)
			: this(length, ValidationKeys.CollectionLength)
        {
        }

	    public CollectionLengthRule(int length, StringToken token)
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
            var enumerable = accessor.GetValue(context.Target) as System.Collections.IEnumerable;
            if (enumerable == null || enumerable.Count() != _length)
            {
                context.Notification.RegisterMessage(accessor, Token, TemplateValue.For(LENGTH, _length));
            }
        }

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((CollectionLengthRule)obj);
		}

		protected bool Equals(CollectionLengthRule other)
		{
			return _length == other._length && Token.Equals(other.Token);
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