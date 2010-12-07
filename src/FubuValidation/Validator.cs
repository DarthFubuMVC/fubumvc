using System;
using FubuCore;
using FubuValidation.Registration;

namespace FubuValidation
{
    public static class Validator
    {
        private static IValidationProvider _provider;
        private static IValidationQuery _model;

        public static IValidationProvider Provider
        {
            get { return _provider; }
        }

        public static IValidationQuery Model
        {
            get { return _model; }
        }

        public static void Initialize<TRegistry>()
            where TRegistry : ValidationRegistry, new()
        {
            Initialize(new TRegistry());
        }

        public static void Initialize(Action<ValidationRegistry> configure)
        {
            Initialize(new ValidationRegistry(configure));
        }

        public static void Initialize(ValidationRegistry registry)
        {
            lock(typeof(Validator))
            {
                _model = registry.BuildQuery();
                _provider = new ValidationProvider(new TypeResolver(), _model);
            }
        }
    }
}