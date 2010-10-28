using FubuValidation.Strategies;

namespace FubuValidation
{
    public class GreaterThanZeroAttribute : FieldMarkerAttribute
    {
        public GreaterThanZeroAttribute()
            : base(typeof(GreaterThanZeroFieldStrategy))
        {
        }
    }
}