using System;

namespace FubuMVC.Validation
{
    public interface IValidationFailureHandler
    {
        void Handle(Type modelType);
    }
}