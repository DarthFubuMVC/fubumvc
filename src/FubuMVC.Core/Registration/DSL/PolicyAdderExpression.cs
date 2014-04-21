using System;
using FubuMVC.Core.Configuration;
using FubuMVC.Core.Registration.Conventions;

namespace FubuMVC.Core.Registration.DSL
{
    public interface IConfigGraph
    {
        void Add(IConfigurationAction action);
        void Reorder(IConfigurationAction action);

    }

    public class PolicyAdderExpression
    {
        protected ConfigGraph Configuration;

        public PolicyAdderExpression(ConfigGraph configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Apply a new behavior re-ordering rule in order to force behaviors to a certain order.
        /// For example, force Authentication behaviors to always be before Authorization behaviors
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public PolicyAdderExpression Reorder(Action<ReorderBehaviorsPolicy> configure)
        {
            var policy = new ReorderBehaviorsPolicy();
            configure(policy);

            Configuration.Add(policy, ConfigurationType.Reordering);

            return this;
        }

        public PolicyAdderExpression Add<T>() where T : IConfigurationAction, new()
        {
            return Add(new T());
        }

        public PolicyAdderExpression Add<T>(Action<T> configure, string configurationType = ConfigurationType.Policy)
            where T : IConfigurationAction, new()
        {
            var action = new T();
            configure(action);

            return Add(action, configurationType);
        }

        public PolicyAdderExpression Add(Action<Policy> configuration,
            string configurationType = ConfigurationType.Policy)
        {
            var policy = new Policy();
            configuration(policy);

            return Add(policy, configurationType);
        }

        public PolicyAdderExpression Add(IConfigurationAction action,
            string configurationType = ConfigurationType.Policy)
        {
            Configuration.Add(action, configurationType);

            return this;
        }

        public void ChainSource<T>() where T : IChainSource, new()
        {
            Configuration.Add(new T());
        }

        public void ChainSource(IChainSource source)
        {
            Configuration.Add(source);
        }
    }
}