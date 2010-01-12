using System;
using System.Collections.Generic;

namespace FubuMVC.Core.Registration.ObjectGraph
{
    public class ObjectDef
    {
        public ObjectDef()
        {
            Dependencies = new List<IDependency>();
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
        public IList<IDependency> Dependencies { get; set; }

        public ObjectDef Child(Type dependencyType, Type actualType)
        {
            var dependency = new ObjectDef(actualType);
            Dependencies.Add(new ConfiguredDependency
            {
                DependencyType = dependencyType,
                Definition = dependency
            });

            return dependency;
        }

        public void Child(Type dependencyType, object value)
        {
            Dependencies.Add(new ValueDependency
            {
                DependencyType = dependencyType,
                Value = value
            });
        }

        public void AcceptVisitor(IDependencyVisitor visitor)
        {
            if (Dependencies != null) Dependencies.Each(x => { x.AcceptVisitor(visitor); });
        }

        public void Child<T>(T value)
        {
            Child(typeof (T), value);
        }

        public override string ToString()
        {
            return string.Format("Name: {0}, Type: {1}, Value: {2}", Name, Type, Value);
        }
    }
}