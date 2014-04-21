using System;
using FubuCore.Formatting;
using FubuMVC.Core.Configuration;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.Registration.DSL
{
    public class PoliciesExpression : PolicyAdderExpression
    {
        public PoliciesExpression(ConfigGraph configuration) : base(configuration)
        {
        }

        public void StringConversions<T>() where T : DisplayConversionRegistry, new()
        {
            var conversions = new T();

            addStringConversions(conversions);
        }


        private void addStringConversions(DisplayConversionRegistry conversions)
        {
            var registry = new ServiceRegistry();
            registry.AddService(typeof (DisplayConversionRegistry), ObjectDef.ForValue(conversions));
            Configuration.Add(registry);
        }

        public void StringConversions(Action<DisplayConversionRegistry> configure)
        {
            var conversions = new DisplayConversionRegistry();
            configure(conversions);

            addStringConversions(conversions);
        }
    }
}