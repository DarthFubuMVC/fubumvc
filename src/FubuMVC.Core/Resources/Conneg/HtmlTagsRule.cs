using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Nodes;
using HtmlTags;

namespace FubuMVC.Core.Resources.Conneg
{
    public class HtmlTagsRule : ConnegRule, DescribesItself
    {
        protected override DoNext applyOutputs(IOutputNode node, BehaviorChain chain, ConnegSettings settings)
        {
            if (chain.ResourceType().CanBeCastTo<HtmlTag>() || chain.ResourceType().CanBeCastTo<HtmlDocument>())
            {
                node.Add(typeof(HtmlStringWriter<>));
                return DoNext.Stop;
            }

            return DoNext.Continue;
        }

        public void Describe(Description description)
        {
            description.ShortDescription =
                "If an action returns a model type that inherits from either HtmlTag or HtmlDocument, the chain will only output HTML";
        }
    }
}