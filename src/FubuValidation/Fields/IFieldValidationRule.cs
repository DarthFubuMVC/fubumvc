using FubuCore.Reflection;

namespace FubuValidation.Fields
{
    public interface IFieldValidationRule
    {
        void Validate(Accessor accessor, ValidationContext context);
    }
}