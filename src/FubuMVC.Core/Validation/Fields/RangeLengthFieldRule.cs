using System.Collections.Generic;
using FubuCore;
using FubuCore.Descriptions;
using FubuCore.Reflection;
using FubuMVC.Core.Localization;

namespace FubuMVC.Core.Validation.Fields
{
    public class RangeLengthFieldRule : IFieldValidationRule, DescribesItself
    {
	    private readonly int _min;
        private readonly int _max;

        public RangeLengthFieldRule(int min, int max)
			: this(min, max, ValidationKeys.RangeLength)
        {
        }

	    public RangeLengthFieldRule(int min, int max, StringToken token)
	    {
		    _min = min;
		    _max = max;
		    Token = token;
	    }

	    public StringToken Token { get; set; }

		public ValidationMode Mode { get; set; }

        public void Validate(Accessor accessor, ValidationContext context)
        {
            var value = context.GetFieldValue<string>(accessor) ?? string.Empty;
            var length = value.Length;

            if(length < _min || length > _max)
            {
                var min = TemplateValue.For("min", _min);
                var max = TemplateValue.For("max", _max);

                context.Notification.RegisterMessage(accessor, Token, min, max);
            }
        }

        public IDictionary<string, object> ToValues()
        {
            return new Dictionary<string, object>
                   {
                       { "min", _min }, { "max", _max }
                   };
        }

		protected bool Equals(RangeLengthFieldRule other)
		{
			return _min == other._min && _max == other._max && Token.Equals(other.Token);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((RangeLengthFieldRule)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = _min;
				hashCode = (hashCode * 397) ^ _max;
				hashCode = (hashCode * 397) ^ Token.GetHashCode();
				return hashCode;
			}
		}

	    public void Describe(Description description)
	    {
		    description.ShortDescription = "Min: {0}; Max: {1}; Message: {2}".ToFormat(_min, _max, Token);
	    }
    }
}