using System;
using FubuCore;
using FubuMVC.Core;
using FubuValidation;

namespace FubuMVC.Validation
{
    public static class FubuRegistryExtensions
    {
        public static void WithValidationDefaults(this FubuRegistry registry)
        {
            registry.Validation<ValidationRegistry>(validation => { });
        }

        public static void Validation<TRegistry>(this FubuRegistry registry, Action<ConfigureValidationExpression> configure)
            where TRegistry : ValidationRegistry, new()
        {
            registry.Validation(new TRegistry(), configure);
        }

        public static void Validation(this FubuRegistry registry, ValidationRegistry validationRegistry, Action<ConfigureValidationExpression> configure)
        {
            var configured = false;
            registry
                .Services(x =>
                              {
                                  configured = x.DefaultServiceFor<IValidationProvider>() != null;
                              });

            // Don't override existing configuration
            if(configured)
            {
                return;
            }

            var query = validationRegistry.BuildQuery();
            var provider = new ValidationProvider(new TypeResolver(), query);

            registry
                .Services(x =>
                              {
                                  x.SetServiceIfNone(query);
                                  x.SetServiceIfNone<IValidationProvider>(provider);
                              });

            var expression = new ConfigureValidationExpression();
            configure(expression);

            expression.ConfigureRegistry(registry);
        }
    }
}