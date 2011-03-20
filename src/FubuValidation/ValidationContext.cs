using System;
using System.ComponentModel;
using FubuCore.Reflection;

namespace FubuValidation
{
    public class ValidationContext
    {
        private readonly Notification _notification;
        private readonly IValidationProvider _provider;
        private readonly object _target;

        public ValidationContext(IValidationProvider provider, Notification notification, object target)
        {
            if (target == null) throw new ArgumentNullException("target");
            if (notification == null) throw new ArgumentNullException("notification");

            _provider = provider;
            _notification = notification;
            _target = target;
        }

        public IValidationProvider Provider
        {
            get { return _provider; }
        }

        public Notification Notification
        {
            get { return _notification; }
        }

        public object Target
        {
            get { return _target; }
        }

        public T GetFieldValue<T>(Accessor accessor)
        {
            var rawValue = accessor.GetValue(_target);
            if (rawValue.GetType() == typeof (T)) return (T) rawValue;

            var converter = TypeDescriptor.GetConverter(typeof (T));
            return (T) converter.ConvertFrom(rawValue);
        }
    }
}