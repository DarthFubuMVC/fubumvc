using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Formatting;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Configuration;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Security;

namespace FubuMVC.Core.Registration.DSL
{


    public class PoliciesExpression 
    {
        private readonly ConfigGraph _configuration;

        public PoliciesExpression(ConfigGraph configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Apply a new behavior re-ordering rule in order to force behaviors to a certain order.
        /// For example, force Authentication behaviors to always be before Authorization behaviors
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public PoliciesExpression Reorder(Action<ReorderBehaviorsPolicy> configure)
        {
            var policy = new ReorderBehaviorsPolicy();
            configure(policy);

            _configuration.Add(policy, ConfigurationType.Reordering);

            return this;
        }

        public void WrapWith<T>() where T : IActionBehavior
        {
            var policy = new Policy();
            policy.Wrap.WithBehavior<T>();

            Add(policy, ConfigurationType.Policy);
        }

        public void Add<T>() where T : IConfigurationAction, new()
        {
            Add(new T(), ConfigurationType.Policy);
        }

        public void Add<T>(Action<T> configure, string configurationType = null) where T : IConfigurationAction, new()
        {
            var action = new T();
            configure(action);

            Add(action, configurationType);
        }

        public void Add(Action<Policy> configuration, string configurationType = ConfigurationType.Policy)
        {
            var policy = new Policy();
            configuration(policy);

            Add(policy, configurationType);
        }

        public void Add(IConfigurationAction action, string configurationType = null)
        {
            _configuration.Add(action, configurationType);
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
            _configuration.Add(registry);
        }

        public void StringConversions(Action<DisplayConversionRegistry> configure)
        {
            var conversions = new DisplayConversionRegistry();
            configure(conversions);

            addStringConversions(conversions);
        }
    }
}