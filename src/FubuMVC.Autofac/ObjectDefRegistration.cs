using System;
using System.Linq;
using System.Reflection;

using Autofac;
using Autofac.Builder;
using Autofac.Core;

using FubuCore;

using FubuMVC.Core.Registration.ObjectGraph;


namespace FubuMVC.Autofac {
	public class ObjectDefRegistration : IDependencyVisitor {
		private readonly ContainerBuilder _builder;
		private readonly ObjectDef _definition;
		private readonly bool _isSingleton;

		private IRegistrationBuilder<object, ReflectionActivatorData, object> _registration;


		public ObjectDefRegistration(ContainerBuilder builder, ObjectDef definition, bool isSingleton) {
			_builder = builder;
			_definition = definition;
			_isSingleton = isSingleton;
		}


		public void Register(Type abstraction) {
			if (_definition.Value != null) {
				// By instance.
				UpdateRegistration(_builder.Register(context => _definition.Value), abstraction, _definition.Name, _isSingleton);
			} else {
				// By type.
				if (_definition.Type.IsOpenGeneric()) {
					_registration = _builder.RegisterGeneric(_definition.Type);
					UpdateRegistration(_registration, abstraction, _definition.Name, _isSingleton);
				} else {
					_registration = _builder.RegisterType(_definition.Type);
					UpdateRegistration(_registration, abstraction, _definition.Name, _isSingleton);
				}

				_definition.AcceptVisitor(this);
			}
		}


		private static void UpdateRegistration<TActivatorData, TRegistrationStyle>(IRegistrationBuilder<object, TActivatorData, TRegistrationStyle> registration, Type type, string name, bool isSingleton) {
			registration.As(type);

			if (name != null) {
				registration.Named(name, type);
			}

			if (isSingleton) {
				registration.SingleInstance();
			} else {
				registration.InstancePerLifetimeScope();
			}
		}


		void IDependencyVisitor.Value(ValueDependency dependency) {
			SetDependencyValue(dependency.DependencyType, dependency.Value);
		}

		void IDependencyVisitor.Configured(ConfiguredDependency dependency) {
			if (dependency.Definition.Value != null) {
				SetDependencyValue(dependency.DependencyType, dependency.Definition.Value);
			} else {
				var registration = new ObjectDefRegistration(_builder, dependency.Definition, false);
				registration.Register(dependency.DependencyType);
			}
		}

		void IDependencyVisitor.List(ListDependency dependency) {
			throw new NotImplementedException();
		}


		private void SetDependencyValue(Type dependencyType, object value) {
			ParameterInfo parameter = FindFirstConstructorParameterOfType(dependencyType);
			if (parameter != null) {
				_registration.WithParameter(parameter.Name, value);
				return;
			}

			PropertyInfo property = FindFirstWriteablePropertyOfType(dependencyType);
			if (property != null) {
				_registration.WithProperty(property.Name, value);
				return;
			}

			throw new DependencyResolutionException("Explicit dependency could not be found");
		}
		
		private ParameterInfo FindFirstConstructorParameterOfType(Type dependencyType) {
			// Get the constructors and their parameters.
			Type targetType = _registration.ActivatorData.ImplementationType;
			ConstructorInfo[] constructors = _registration.ActivatorData.ConstructorFinder.FindConstructors(targetType);
			var parametersByConstructor = constructors.Select(ctor => ctor.GetParameters()).ToList();

			// Find the set(s) of parameters that are the longest.
			// NOTE: This may not be ideal, but Autofac does not seem to surface the chosen constructor.
			int maxParameterLength = parametersByConstructor.Max(l => l.Length);
			var parameterCandidates = parametersByConstructor.Where(l => l.Length == maxParameterLength).SelectMany(l => l);

			// Look for a constructor parameter matching the dependency type.
			return parameterCandidates.FirstOrDefault(p => p.ParameterType == dependencyType);
		}

		private PropertyInfo FindFirstWriteablePropertyOfType(Type dependencyType) {
			// Get the properties.
			Type targetType = _registration.ActivatorData.ImplementationType;
			PropertyInfo[] properties = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

			// Look for a property that is writeable and whose type matches the dependency type.
			return properties.FirstOrDefault(p => p.CanWrite && p.PropertyType == dependencyType);
		}
	}
}