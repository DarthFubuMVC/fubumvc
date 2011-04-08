using System;
using FubuCore;

namespace FubuMVC.Core.Registration.ObjectGraph
{
    public class ValueDependency : IDependency
    {
        public object Value { get; private set; }

        public ValueDependency(Type dependencyType, object value)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (dependencyType == null) throw new ArgumentNullException("dependencyType");

            if (!value.GetType().CanBeCastTo(dependencyType))
            {
                throw new ObjectDefException("{0} cannot be cast to dependency type {1}", value, dependencyType.FullName);
            }

            Value = value;
            DependencyType = dependencyType;
        }

        public void AcceptVisitor(IDependencyVisitor visitor)
        {
            visitor.Value(this);
        }

        public void AssertValid()
        {
        }

        public Type DependencyType { get; private set; }

        public override string ToString()
        {
            return string.Format("DependencyType: {1}, Value: {0}", Value, DependencyType);
        }
    }
}