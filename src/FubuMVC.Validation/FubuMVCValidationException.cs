using System;
using FubuMVC.Core;
using FubuValidation;

namespace FubuMVC.Validation
{
    public class FubuMVCValidationException : FubuException
    {
        [NonSerialized]
        private readonly Notification _notification;

        public FubuMVCValidationException(int errorCode, Notification notification, string message) 
            : base(errorCode, message)
        {
            _notification = notification;
        }

        public FubuMVCValidationException(int errorCode, Notification notification, string template, params string[] substitutions) 
            : base(errorCode, template, substitutions)
        {
            _notification = notification;
        }

        public Notification Notification { get { return _notification; } }
    }
}