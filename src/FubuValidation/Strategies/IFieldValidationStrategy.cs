using System.Collections.Specialized;
using FubuCore.Reflection;

namespace FubuValidation.Strategies
{
    public interface IFieldValidationStrategy
    {
        NameValueCollection GetMessageSubstitutions(Accessor accessor);
        ValidationStrategyResult Validate(ValidationStrategyContext context);
    }
}