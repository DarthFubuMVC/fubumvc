using FubuCore;
using FubuValidation.Strategies;

namespace FubuValidation
{
    public class CollectionLengthAttribute : FieldMarkerAttribute
    {
        private readonly int _length;

        public CollectionLengthAttribute(int length) 
            : base(typeof(CollectionLengthValidationStrategy))
        {
            _length = length;
        }

        public int Length
        {
            get { return _length; }
        }

        protected override void visitStrategy(IFieldValidationStrategy strategy)
        {
            strategy
                .As<CollectionLengthValidationStrategy>()
                .Length = Length;
        }
    }
}