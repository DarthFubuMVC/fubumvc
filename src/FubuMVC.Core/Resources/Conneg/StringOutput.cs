using FubuCore.Descriptions;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Resources.Conneg
{
    public class StringOutput : ConnegRule, DescribesItself
    {
        protected override DoNext applyOutputs(IOutputNode node, BehaviorChain chain, ConnegSettings settings)
        {
            if (chain.ResourceType() == typeof (string))
            {
                node.Add(new StringWriter());

                return DoNext.Stop;
            }

            return base.applyOutputs(node, chain, settings);
        }

        public void Describe(Description description)
        {
            description.ShortDescription = "If an action returns a .Net string, the chain will only render text/plain";
        }
    }
}