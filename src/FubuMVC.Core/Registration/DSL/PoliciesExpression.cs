using System;
using FubuCore.Formatting;
using FubuMVC.Core.Configuration;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.Registration.DSL
{
    public class PoliciesExpression : PolicyAdderExpression
    {
        private readonly ConfigGraph _graph;

        public PoliciesExpression(ConfigGraph configuration) : base(configuration.Global)
        {
            _graph = configuration;
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
            _graph.Add(registry);
        }

        public void StringConversions(Action<DisplayConversionRegistry> configure)
        {
            var conversions = new DisplayConversionRegistry();
            configure(conversions);

            addStringConversions(conversions);
        }

        public void ChainSource<T>() where T : IChainSource, new()
        {
            _graph.Add(new T());
        }

        public void ChainSource(IChainSource source)
        {
            _graph.Add(source);
        }
    }
}