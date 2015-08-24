using System.Linq;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Resources.Conneg
{
    public class ViewAttachment : ConnegRule
    {
        protected override DoNext applyOutputs(IOutputNode node, BehaviorChain chain, ConnegSettings settings)
        {
            var candidates = settings.Views.Where(x => x.ViewModel == node.ResourceType).ToArray();
            if (candidates.Count() == 1)
            {
                if (!node.HasView())
                {
                    node.AddView(candidates.Single());
                }
            }

            return DoNext.Continue;
        }

    }
}