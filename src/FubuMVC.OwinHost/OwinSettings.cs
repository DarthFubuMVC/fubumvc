using System.Collections.Generic;
using FubuCore.Util;
using FubuMVC.Core.Registration;
using Owin;

namespace FubuMVC.OwinHost
{
    [ApplicationLevel]
    public class OwinSettings : IAppBuilderConfiguration
    {
        /// <summary>
        /// Key value pairs to control or alter the behavior of the underlying host
        /// </summary>
        public readonly IDictionary<string, object> Properties = new Dictionary<string, object>();


        /// <summary>
        /// A list of keys and their associated values that will be injected by the host into each OWIN request environment.
        /// </summary>
        public readonly Cache<string, object> EnvironmentData = new Cache<string, object>();

        void IAppBuilderConfiguration.Configure(IAppBuilder builder)
        {
            EnvironmentData.Each((key, value) => {
                if (builder.Properties.ContainsKey(key))
                {
                    builder.Properties[key] = value;
                }
                else
                {
                    builder.Properties.Add(key, value);
                }
            });
        }
    }

    public interface IAppBuilderConfiguration
    {
        void Configure(IAppBuilder builder);
    }
}