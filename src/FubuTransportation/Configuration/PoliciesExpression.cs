using System;
using System.Collections.Generic;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuTransportation.Configuration
{
    public class PoliciesExpression
    {
        private readonly Action<IConfigurationAction> _registration;

        public PoliciesExpression(Action<IConfigurationAction> registration)
        {
            _registration = registration;
        }

        /// <summary>
        ///  Registers a new Policy
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public PoliciesExpression Policy<T>() where T : IConfigurationAction, new()
        {
            _registration(new T());
            return this;
        }

        /// <summary>
        /// Wrap a behavior of type T around all HandlerChain's
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void WrapWith<T>() where T : IActionBehavior
        {
            var policy = new Policy();
            policy.Wrap.WithBehavior<T>();

            _registration(policy);
        }
    }
}