using FubuCore;
using FubuValidation.Strategies;

namespace FubuValidation
{
    public class MaximumStringLengthAttribute : FieldMarkerAttribute
    {
        private readonly int _length;

        public MaximumStringLengthAttribute(int length)
            : base(typeof(MaximumStringLengthFieldStrategy))
        {
            _length = length;
        }

        public int Length
        {
            get { return _length; }
        }

        protected override void visitStrategy(IFieldValidationStrategy strategy)
        {
            // attributes don't allow generics...
            strategy
                .As<MaximumStringLengthFieldStrategy>()
                .Length = Length;
        }
    }
}