using System;
using System.Collections.Generic;
using System.Reflection;
using Bottles;
using FubuCore.Reflection;
using FubuFastPack.JqGrid;
using FubuFastPack.NHibernate;
using FubuFastPack.Querying;
using FubuFastPack.Validation;
using FubuMVC.Core.Packaging;
using FubuValidation;
using FubuValidation.Fields;
using StructureMap;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using FubuCore;

namespace FubuFastPack.StructureMap
{
    public class FastPackRegistry : Registry
    {
        public FastPackRegistry()
        {
            For(typeof (IGridRunner<,>)).Use(typeof (GridRunner<,>));
            For<IQueryService>().Use<QueryService>();
            For(typeof (Projection<>)).LifecycleIs(InstanceScope.Unique).Use(typeof (Projection<>));


        }
    }

    public enum IncludePackageAssemblies
    {
        Yes,
        No
    }

    public static class ValidationRegistryExtensions
    {
        public static void FubuValidationWith(this Registry registry, IncludePackageAssemblies packageAssemblies, params Assembly[] assemblies)
        {
            registry.ForSingletonOf<ITypeDescriptorCache>().Use<TypeDescriptorCache>();
            registry.For<IValidator>().Use<Validator>();
            registry.For<IValidationSource>().Add<UniqueValidationSource>();
            registry.ForSingletonOf<IFieldRulesRegistry>().Add<FieldRulesRegistry>();
            registry.For<IValidationSource>().Add<FieldRuleSource>();

            var convention = new ValidationConvention();
            registry.Scan(x =>
            {
                assemblies.Each(x.Assembly);
                if (packageAssemblies == IncludePackageAssemblies.Yes)
                {
                    PackageRegistry.PackageAssemblies.Each(x.Assembly);
                }

                x.With(convention);
            });

            registry.Activate<IFieldRulesRegistry>("Applying explicit field validtion rules", fieldRules =>
            {
                convention.Registrations.Each(x => x.RegisterFieldRules(fieldRules));
            });
        }
    }

    public class ValidationConvention : IRegistrationConvention
    {
        private readonly IList<IValidationRegistration> _registrations = new List<IValidationRegistration>();

        public IList<IValidationRegistration> Registrations
        {
            get { return _registrations; }
        }

        public void Process(Type type, Registry registry)
        {
            if (type.CanBeCastTo<IValidationRegistration>() && type.IsConcreteWithDefaultCtor())
            {
                var registration = (IValidationRegistration)Activator.CreateInstance(type);
                _registrations.Add(registration);

                registration.FieldSources().Each(x => registry.For<IFieldValidationSource>().Add(x));
            }
        }
    }
}