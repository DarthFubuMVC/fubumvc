using System;
using System.Collections.Generic;
using FubuMVC.Core;
using FubuValidation;
using FubuValidation.Registration;

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
            // Don't override existing configuration
            if(registry.IsValidationConfigured())
            {
                return;
            }

            var sources = validationRegistry.GetConfiguredSources();
            registry
                .Services(x =>
                              {
                                  sources.Each(x.AddService<IValidationSource>);
                                  //x.SetServiceIfNone<IValidationQuery, ValidationQuery>();
                                  x.SetServiceIfNone<IValidationProvider, ValidationProvider>();
                              });

            var expression = new ConfigureValidationExpression();
            configure(expression);

            expression.ConfigureRegistry(registry);
        }

        public static bool IsValidationConfigured(this FubuRegistry registry)
        {
            // TODO -- There's GOT to be a cleaner way to do this
            var configured = false;
            registry
                .Services(x =>
                              {
                                  configured = x.DefaultServiceFor<IValidationProvider>() != null;
                              });

            return configured;
        }
    }
}