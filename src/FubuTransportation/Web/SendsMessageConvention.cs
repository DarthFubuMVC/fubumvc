using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Registration;

namespace FubuTransportation.Web
{
    public class SendsMessageConvention : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            var transformers = graph.FirstActions().Where(x => x.HandlerType.CanBeCastTo<ISendMessages>());
            transformers.Each(x => {
                x.AddAfter(new SendsMessage(x));
                var chain = x.ParentChain();
                chain.ResourceType(typeof (AjaxContinuation));
            });
        }
    }
}