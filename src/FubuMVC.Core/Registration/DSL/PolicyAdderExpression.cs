using System;
using FubuMVC.Core.Http.Compression;
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
        public readonly static HttpContentEncodingFilter DefaultFilter = new HttpContentEncodingFilter(new HttpContentEncoders(new IHttpContentEncoding[] { new GZipHttpContentEncoding(), new DeflateHttpContentEncoding() }));


        protected PolicyGraph Configuration;

        public PolicyAdderExpression(PolicyGraph configuration)
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

            Configuration.Reordering.Fill(policy);

            return this;
        }

        public PolicyAdderExpression Add<T>() where T : IConfigurationAction, new()
        {
            return Add(new T());
        }

        public PolicyAdderExpression Add<T>(Action<T> configure)
            where T : IConfigurationAction, new()
        {
            var action = new T();
            configure(action);

            return Add(action);
        }

        public PolicyAdderExpression Configure(Action<BehaviorGraph> configuration)
        {
            var action = new LambdaConfigurationAction(configuration);
            return Add(action);
        }


        public PolicyAdderExpression Add(IConfigurationAction action)
        {
            Configuration.Policies.Fill(action);

            return this;
        }
    }
}