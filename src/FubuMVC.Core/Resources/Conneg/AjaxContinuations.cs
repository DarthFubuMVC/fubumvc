using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Resources.Conneg
{
    public class AjaxContinuations : ConnegRule, DescribesItself
    {
        protected override DoNext applyInputs(IInputNode node, BehaviorChain chain, ConnegSettings settings)
        {
            if (!chain.ResourceType().CanBeCastTo<AjaxContinuation>()) return DoNext.Continue;

            node.Add(typeof(ModelBindingReader<>));
            node.Add(settings.FormatterFor(MimeType.Json));

            return DoNext.Stop;
        }

        protected override DoNext applyOutputs(IOutputNode node, BehaviorChain chain, ConnegSettings settings)
        {
            if (!chain.ResourceType().CanBeCastTo<AjaxContinuation>()) return DoNext.Continue;
       
            node.Add(typeof(AjaxContinuationWriter<>));

            return DoNext.Stop;
        }

        public void Describe(Description description)
        {
            description.ShortDescription =
                "If any action returns AjaxContinuation or a subtype, accept Json or HTTP form posts and only output Json with the AjaxContinuationWriter";
        }
    }
}