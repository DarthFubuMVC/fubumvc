using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration;

namespace FubuTransportation.Configuration
{
    /// <summary>
    /// Base class for creating policies against HandlerChain's
    /// </summary>
    public abstract class HandlerChainPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Behaviors.OfType<HandlerChain>().Where(Matches).Each(Configure);
        }

        /// <summary>
        /// Override this for the alteration to each HandlerChain
        /// </summary>
        /// <param name="handlerChain"></param>
        public abstract void Configure(HandlerChain handlerChain);

        /// <summary>
        /// Override this to control the applicability of this policy to 
        /// each chain.  Default is to always apply
        /// </summary>
        /// <param name="chain"></param>
        /// <returns></returns>
        public virtual bool Matches(HandlerChain chain)
        {
            return true;
        }
    }
}