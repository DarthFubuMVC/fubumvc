using System;
using FubuValidation;

namespace FubuMVC.Validation
{
    public interface IValidationFailurePolicy
    {
        bool Matches(Type modelType);
        void Handle(Type modelType, Notification notification);
    }
}