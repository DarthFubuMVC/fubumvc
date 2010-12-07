using FubuValidation.Strategies;

namespace FubuValidation
{
    public class RequiredAttribute : FieldMarkerAttribute
    {
        public RequiredAttribute()
            : base(typeof(RequiredFieldStrategy))
        {
        }
    }
}