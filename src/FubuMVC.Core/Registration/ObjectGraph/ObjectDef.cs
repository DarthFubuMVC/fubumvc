using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Security;

namespace FubuMVC.Core.Registration.ObjectGraph
{
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


        public string Name { get; set; }
        public Type Type { get; set; }
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
            _dependencies.Add(new ConfiguredDependency{
                DependencyType = dependencyType,
                Definition = dependency
            });
        }

        public void Dependency(IDependency dependency)
        {
            _dependencies.Add(dependency);
        }

        public void DependencyByValue(Type dependencyType, object value)
        {
            _dependencies.Add(new ValueDependency{
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
            var dependency = _dependencies.OfType<ListDependency>().FirstOrDefault(x => x.DependencyType == typeof(IEnumerable<T>));
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
            return new ObjectDef(){
                Value = value
            };
        }
    }
}