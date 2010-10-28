using FubuValidation.Strategies;

namespace FubuValidation
{
    public class GreaterOrEqualToZeroAttribute : FieldMarkerAttribute
    {
        public GreaterOrEqualToZeroAttribute()
            : base(typeof(GreaterOrEqualToZeroFieldStrategy))
        {
        }
    }
}