using System;
using System.Collections.Generic;
using FubuMVC.UI.Scripts.Registration.Conventions;
using FubuMVC.UI.Scripts.Registration.DSL;

namespace FubuMVC.UI.Scripts.Registration
{
    public class ScriptRegistry
    {
        private readonly FilePool _files = new FilePool();
        private readonly List<IScriptConfigurationAction> _actions = new List<IScriptConfigurationAction>();
        public AppliesToDirectoryExpression Applies { get { return new AppliesToDirectoryExpression(_files); } }

        public ScriptRegistry()
        {
            ApplyConvention<DefaultScriptRegistrationConvention>();
        }

        public ScriptRegistry(Action<ScriptRegistry> configure)
            : this()
        {
            configure(this);
        }

        public void ApplyConvention<TConvention>()
            where TConvention : class, IScriptConfigurationAction, new()
        {
            ApplyConvention(new TConvention());
        }

        public void ApplyConvention<TConvention>(TConvention convention)
            where TConvention : IScriptConfigurationAction
        {
            _actions.Add(convention);
        }

        public ScriptGraph BuildGraph()
        {
            var graph = new ScriptGraph();
            _actions.Configure(_files, graph);
            return graph;
        }
    }
}