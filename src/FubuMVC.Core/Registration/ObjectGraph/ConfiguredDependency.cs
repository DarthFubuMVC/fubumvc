using System;

namespace FubuMVC.Core.Registration.ObjectGraph
{
    public class ConfiguredDependency : IDependency
    {
        public ObjectDef Definition { get; private set; }
        public Type DependencyType { get; private set; }

        public ConfiguredDependency(Type dependencyType, ObjectDef definition)
        {
            if (dependencyType == null) throw new ArgumentNullException("dependencyType");

            DependencyType = dependencyType;
            Definition = definition;
        }

        public ConfiguredDependency(Type dependencyType, Type concreteType) : this(dependencyType, new ObjectDef(concreteType))
        {
        }

        public ConfiguredDependency(Type dependencyType, object value) : this(dependencyType, ObjectDef.ForValue(value)){}


        public void AcceptVisitor(IDependencyVisitor visitor)
        {
            visitor.Configured(this);
        }

        public void AssertValid()
        {
            Definition.ValidatePluggabilityTo(DependencyType);
        }

        public override string ToString()
        {
            return string.Format("DependencyType: {0}, Definition: {1}", DependencyType, Definition);
        }

        public static ConfiguredDependency For<TDependency, TConcrete>() where TConcrete : TDependency
        {
            return new ConfiguredDependency(typeof(TDependency), ObjectDef.ForType<TConcrete>());
        }
    }
    
}