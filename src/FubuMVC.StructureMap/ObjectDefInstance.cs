using System;
using FubuMVC.Core.Registration.ObjectGraph;
using StructureMap.Pipeline;
using System.Linq;

namespace FubuMVC.StructureMap
{
    public class ObjectDefInstance : ConfiguredInstance, IDependencyVisitor
    {
        public ObjectDefInstance(ObjectDef definition)
            : base(definition.Type)
        {
            definition.AcceptVisitor(this);
            Name = definition.Name;
        }

        void IDependencyVisitor.Value(ValueDependency dependency)
        {
            Child(dependency.DependencyType).Is(dependency.Value);
        }

        void IDependencyVisitor.Configured(ConfiguredDependency dependency)
        {
            var child = new ObjectDefInstance(dependency.Definition);
            Child(dependency.DependencyType).Is(child);
        }

        void IDependencyVisitor.List(ListDependency dependency)
        {
            var elements = dependency.Items.Select(instanceFor).ToArray();

            ChildArray(dependency.DependencyType).Contains(elements);
        }

        private Instance instanceFor(ObjectDef def)
        {
            return def.Value != null 
                ? (Instance) new ObjectInstance(def.Value) 
                : new ObjectDefInstance(def);
        }
    }
}