using FubuCore.Reflection;

namespace FubuValidation.Fields
{
    public class MaximumLengthRule : IFieldValidationRule
    {
        public static readonly string LENGTH = "length";
        private readonly int _length;

        public MaximumLengthRule(int length)
        {
            _length = length;
        }

        public int Length
        {
            get { return _length; }
        }

        public void Validate(Accessor accessor, ValidationContext context)
        {
            var rawValue = accessor.GetValue(context.Target);
            if (rawValue != null && rawValue.ToString().Length > Length)
            {
                context.Notification.RegisterMessage(accessor, ValidationKeys.MAX_LENGTH)
                    .AddSubstitution(LENGTH, _length.ToString());
            }
        }

        public bool Equals(MaximumLengthRule other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other._length == _length;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (MaximumLengthRule)) return false;
            return Equals((MaximumLengthRule) obj);
        }

        public override int GetHashCode()
        {
            return _length;
        }
    }
}