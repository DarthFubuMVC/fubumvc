using System;
using FubuCore.Formatting;
using FubuMVC.Core.Configuration;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.Registration.DSL
{
    public class PoliciesExpression
    {
        private readonly ConfigGraph _graph;

        public PoliciesExpression(ConfigGraph configuration) 
        {
            _graph = configuration;
        }

        public PolicyAdderExpression Global
        {
            get
            {
                return new PolicyAdderExpression(_graph.Global);
            }
        }

        public PolicyAdderExpression Local
        {
            get
            {
                return new PolicyAdderExpression(_graph.Local);
            }
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