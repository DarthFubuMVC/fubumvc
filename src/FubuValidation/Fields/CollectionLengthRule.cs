using FubuCore.Reflection;

namespace FubuValidation.Fields
{
    public class CollectionLengthRule : IFieldValidationRule
    {
        private readonly int _length;
        public static readonly string LENGTH = "length";

        public CollectionLengthRule(int length)
        {
            _length = length;
        }

        public void Validate(Accessor accessor, ValidationContext context)
        {
            var enumerable = accessor.GetValue(context.Target) as System.Collections.IEnumerable;
            if (enumerable == null || enumerable.Count() != _length)
            {
                context.Notification.RegisterMessage(accessor, ValidationKeys.COLLECTION_LENGTH).AddSubstitution(LENGTH, _length.ToString());
            }
        }

        public int Length
        {
            get { return _length; }
        }
    }
}