using FubuCore.Reflection;
using FubuValidation.Registration;
using FubuValidation.Strategies;

namespace FubuValidation
{
    public static class ValidationQueryExtensions
    {
        public static bool IsRequired(this IValidationQuery query, Accessor accessor)
        {
            return query.HasStrategy<RequiredFieldStrategy>(accessor);
        }
    }
}