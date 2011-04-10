using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using FubuCore;

namespace FubuMVC.Core.Registration.ObjectGraph
{
    [Serializable]
    public class ObjectDefException : Exception
    {
        public ObjectDefException(string message, params object[] parameters) : base(message.ToFormat(parameters))
        {
        }

        protected ObjectDefException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    /// <summary>
    /// IoC container-agnostic model of a service or configured object.  The
    /// equivalent of a StructureMap "Instance" or a Windsor "Component" 
    /// </summary>
    public class ObjectDef
    {
        private readonly IList<IDependency> _dependencies;

        public ObjectDef()
        {
            _dependencies = new List<IDependency>();
            Name = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Optional parameterTypes can be used to close type
        /// if it is an open generic type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parameterTypes"></param>
        public ObjectDef(Type type, params Type[] parameterTypes)
            : this()
        {

            if (type == null)
            {
                throw new ObjectDefException("type cannot be null in this usage");
            }

            Type = type.IsOpenGeneric() && parameterTypes.Any()
                       ? type.MakeGenericType(parameterTypes)
                       : type;

        }

        /// <summary>
        /// The name for this configured object when it is registered into the underlying IoC 
        /// container
        /// </summary>
        public string Name { get; set; }

        private Type _type;

        /// <summary>
        /// The concrete type of this ObjectDef.  Set this property if the IoC container 
        /// should build a new object instance on each request
        /// </summary>
        public Type Type
        {
            get { return _type; }
            set
            {
                if (value != null && !value.IsConcrete())
                {
                    throw new ObjectDefException("{0} is not a concrete type.", value.FullName);
                }

                _value = null;
                _type = value;
            }
        }

        private object _value;

        /// <summary>
        /// The actual object instance that the IoC container will resolve.  This property is mutually
        /// exclusive to the Type property
        /// </summary>
        public object Value
        {
            get { return _value; }
            set
            {
                _type = null;
                _dependencies.Clear();
                _value = value;
            }
        }

        /// <summary>
        /// All the explicitly configured dependencies of this ObjectDef
        /// </summary>
        public IEnumerable<IDependency> Dependencies
        {
            get { return _dependencies; }
        }

        /// <summary>
        /// Creates a new ObjectDef for type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ObjectDef ForType<T>()
        {
            return new ObjectDef(typeof (T));
        }

        /// <summary>
        /// Creates and configures a new ObjectDef for type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static ObjectDef ForType<T>(Action<ObjectDef> configure)
        {
            var objectDef = new ObjectDef(typeof (T));
            configure(objectDef);

            return objectDef;
        }

        /// <summary>
        /// Registers a configured dependency of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dependency"></param>
        public void DependencyByType<T>(ObjectDef dependency)
        {
            Dependency(typeof (T), dependency);
        }

        /// <summary>
        /// Registers a specific concrete type for the dependencyType
        /// </summary>
        /// <param name="dependencyType"></param>
        /// <param name="actualType"></param>
        /// <returns></returns>
        public ObjectDef DependencyByType(Type dependencyType, Type actualType)
        {
            if (!actualType.IsConcrete())
            {
                throw new ObjectDefException("actualType must be concrete ({0})", actualType.FullName);
            }

            var dependency = new ObjectDef(actualType);
            Dependency(dependencyType, dependency);

            return dependency;
        }

        /// <summary>
        /// Registers a configured dependency for the dependencyType
        /// </summary>
        /// <param name="dependencyType"></param>
        /// <param name="dependency"></param>
        public void Dependency(Type dependencyType, ObjectDef dependency)
        {
            var configuredDependency = new ConfiguredDependency(dependencyType, dependency);

            Dependency(configuredDependency);
        }


        public void Dependency(IDependency dependency)
        {
            dependency.AssertValid();
            _dependencies.Add(dependency);
        }

        /// <summary>
        /// Register a prebuilt value for the dependencyType of this ObjectDef
        /// </summary>
        /// <param name="dependencyType"></param>
        /// <param name="value"></param>
        public void DependencyByValue(Type dependencyType, object value)
        {
            Dependency(new ValueDependency(dependencyType, value));
        }

        public void AcceptVisitor(IDependencyVisitor visitor)
        {
            if (Dependencies != null) Dependencies.Each(x => { x.AcceptVisitor(visitor); });
        }

        /// <summary>
        /// Register a prebuilt value for typeof(T)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        public void DependencyByValue<T>(T value)
        {
            if (value is IDependency)
            {
                throw new ObjectDefException("IDependency objects are not valid in this usage");
            }

            DependencyByValue(typeof (T), value);
        }

        private ListDependency findListDependency<T>(Type openType)
        {
            var dependencyType = openType.MakeGenericType(typeof (T));
            var dependency = _dependencies.OfType<ListDependency>()
                .FirstOrDefault(x => x.DependencyType == typeof (IEnumerable<T>));
            
            if (dependency == null)
            {
                dependency = new ListDependency(dependencyType);
                _dependencies.Add(dependency);
            }

            return dependency;
        }

        /// <summary>
        /// Configure individual members of an IEnumerable<T> dependency
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ListDependency EnumerableDependenciesOf<T>()
        {
            return findListDependency<T>(typeof (IEnumerable<>));
        }

        /// <summary>
        /// Configure individual members of an IList<T> dependency
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ListDependency ListDependenciesOf<T>()
        {
            return findListDependency<T>(typeof (IList<>));
        }

        public override string ToString()
        {
            return string.Format("Name: {0}, Type: {1}, Value: {2}", Name, Type, Value);
        }

        /// <summary>
        /// Creates an ObjectDef for a prebuilt value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ObjectDef ForValue(object value)
        {
            return new ObjectDef{
                Value = value
            };
        }

        public void ValidatePluggabilityTo(Type dependencyType)
        {
            if (Value == null && Type == null)
            {
                throw new ObjectDefException("No value or concrete type was specified for the dependency");
            }

            if (Value != null && !Value.GetType().CanBeCastTo(dependencyType))
            {
                throw new ObjectDefException("Object of type {0} can not be cast to {1}", Value.GetType().FullName, dependencyType.FullName);
            }

            if (Type != null)
            {
                if (!Type.CanBeCastTo(dependencyType))
                {
                    throw new ObjectDefException("{0} cannot be cast to {1}", Type.FullName, dependencyType.FullName);
                }
            }
        }

        /// <summary>
        /// Locates any explicitly registered dependency of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IDependency DependencyFor<T>()
        {
            return Dependencies.FirstOrDefault(x => x.DependencyType == typeof (T));
        }
    }
}