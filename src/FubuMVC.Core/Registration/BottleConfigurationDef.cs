using Bottles;
using Bottles.Configuration;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.Registration
{
    public class BottleConfigurationDef : IServiceGraphAlteration
    {
        private readonly ObjectDef _objectDef;
        private readonly ListDependency _rules;

        public BottleConfigurationDef(string provenance)
        {
            _objectDef = new ObjectDef(typeof(AssertBottleConfiguration));
            _objectDef.DependencyByValue(provenance);

            _rules = _objectDef.EnumerableDependenciesOf<IBottleConfigurationRule>();
        }

        void IServiceGraphAlteration.Alter(ServiceGraph graph)
        {
            graph.AddService(typeof(IActivator), _objectDef);
        }

        public BottleConfigurationDef AddRule<T>() where T : IBottleConfigurationRule
        {
            _rules.AddType(typeof (T));
            return this;
        }

        public BottleConfigurationDef AddRule(IBottleConfigurationRule rule)
        {
            _rules.AddValue(rule);
            return this;
        }
    }
}