using System;
using System.ComponentModel;
using FubuCore;
using FubuCore.Reflection;

namespace FubuMVC.Core.Validation
{
    public class ValidationContext
    {
        private readonly Notification _notification;
        private readonly IValidator _provider;
        private readonly object _target;
        private ITypeResolver _resolver;
        private IServiceLocator _services;

        public ValidationContext(IValidator provider, Notification notification, object target)
        {
            if (target == null) throw new ArgumentNullException("target");
            if (notification == null) throw new ArgumentNullException("notification");

            _provider = provider;
            _notification = notification;
            _target = target;
        }

        public Type TargetType { get; set; }

        public IValidator Provider
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

        public ITypeResolver Resolver
        {
            get { return _resolver ?? new TypeResolver(); }
            set { _resolver = value; }
        }

        public IServiceLocator ServiceLocator
        {
            get
            {
                if(_services == null)
                {
                    _services = new InMemoryServiceLocator();
                }

                return _services;
            }
            set { _services = value; }
        }

        public T GetFieldValue<T>(Accessor accessor)
        {
            var rawValue = accessor.GetValue(_target);
            
            if (rawValue == null) return default(T);
            if (rawValue.GetType() == typeof (T)) return (T) rawValue;

            var converter = TypeDescriptor.GetConverter(typeof (T));
            return (T) converter.ConvertFrom(rawValue);
        }

        
        public void ContinueValidation(Accessor accessor)
        {
            var childTarget = accessor.GetValue(_target);
            if (childTarget == null) return;

            var childNotification = Provider.Validate(childTarget);
            Notification.AddChild(accessor, childNotification);
        }

        public T Service<T>()
        {
            return ServiceLocator.GetInstance<T>();
        }

        /// <summary>
        /// Mostly used for testing.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static ValidationContext For(object target)
        {
            var provider = Validator.BasicValidator();
            var notification = new Notification(target.GetType());

            return new ValidationContext(provider, notification, target);
        }
    }
}