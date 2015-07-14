using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Resources.Conneg
{
    public abstract class ConnegRule : Node<ConnegRule, ConnegRules>
    {
        public void ApplyInputs(IInputNode node, BehaviorChain chain, ConnegSettings settings)
        {
            if (applyInputs(node, chain, settings) == DoNext.Continue && Next != null)
            {
                Next.ApplyInputs(node, chain, settings);
            }
        }

        protected virtual DoNext applyInputs(IInputNode node, BehaviorChain chain, ConnegSettings settings)
        {
            return DoNext.Continue;
        }

        public void ApplyOutputs(IOutputNode node, BehaviorChain chain, ConnegSettings settings)
        {
            if (applyOutputs(node, chain, settings) == DoNext.Continue && Next != null)
            {
                Next.ApplyOutputs(node, chain, settings);
            }
        }

        protected virtual DoNext applyOutputs(IOutputNode node, BehaviorChain chain, ConnegSettings settings)
        {
            return DoNext.Continue;
        }
    }
}