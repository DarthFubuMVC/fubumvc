using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Core.Registration.ObjectGraph
{
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

        public ObjectDef(Type type)
            : this()
        {
            Type = type;
        }

        /// <summary>
        /// The name for this configured object when it is registered into the underlying IoC 
        /// container
        /// </summary>
        public string Name { get; set; }

        // TODO -- Setting the type should clear the Value, and vice versa
        public Type Type { get; set; }

        // TODO -- have a different type of ObjectDef for Value's like StructureMap?
        public object Value { get; set; }

        public IEnumerable<IDependency> Dependencies
        {
            get { return _dependencies; }
        }

        public static ObjectDef ForType<T>()
        {
            return new ObjectDef(typeof (T));
        }

        public static ObjectDef ForType<T>(Action<ObjectDef> configure)
        {
            var objectDef = new ObjectDef(typeof (T));
            configure(objectDef);

            return objectDef;
        }

        public void DependencyByType<T>(ObjectDef dependency)
        {
            Dependency(typeof (T), dependency);
        }

        public ObjectDef DependencyByType(Type dependencyType, Type actualType)
        {
            var dependency = new ObjectDef(actualType);
            Dependency(dependencyType, dependency);

            return dependency;
        }

        public void Dependency(Type dependencyType, ObjectDef dependency)
        {
            var configuredDependency = new ConfiguredDependency{
                DependencyType = dependencyType,
                Definition = dependency
            };

            Dependency(configuredDependency);
        }

        public void Dependency(IDependency dependency)
        {
            // TODO -- validate that this little monkey is valid
            _dependencies.Add(dependency);
        }

        public void DependencyByValue(Type dependencyType, object value)
        {
            Dependency(new ValueDependency{
                DependencyType = dependencyType,
                Value = value
            });
        }

        public void AcceptVisitor(IDependencyVisitor visitor)
        {
            if (Dependencies != null) Dependencies.Each(x => { x.AcceptVisitor(visitor); });
        }

        public void DependencyByValue<T>(T value)
        {
            // TODO -- Throw if this is an IDependency
            DependencyByValue(typeof (T), value);
        }

        private ListDependency findListDependency<T>(Type openType)
        {
            var dependencyType = openType.MakeGenericType(typeof (T));
            var dependency =
                _dependencies.OfType<ListDependency>().FirstOrDefault(x => x.DependencyType == typeof (IEnumerable<T>));
            if (dependency == null)
            {
                dependency = new ListDependency(dependencyType);
                _dependencies.Add(dependency);
            }

            return dependency;
        }

        public ListDependency EnumerableDependenciesOf<T>()
        {
            return findListDependency<T>(typeof (IEnumerable<>));
        }

        public ListDependency ListDependenciesOf<T>()
        {
            return findListDependency<T>(typeof (IList<>));
        }

        public override string ToString()
        {
            return string.Format("Name: {0}, Type: {1}, Value: {2}", Name, Type, Value);
        }

        public static ObjectDef ForValue(object value)
        {
            return new ObjectDef{
                Value = value
            };
        }
    }
}