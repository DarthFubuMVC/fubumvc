using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;

namespace FubuMVC.Core.Registration.Policies
{
    public class ConnegExpression
    {
        private readonly Policy _policy;

        public ConnegExpression(Policy policy)
        {
            _policy = policy;
        }

        public void MakeAsymmetricJson()
        {
            _policy.ModifyWith<AsymmetricJsonModification>();
        }

        public void MakeSymmetricJson()
        {
            _policy.ModifyWith<SymmetricJsonModification>();
        }
    }

    public class AsymmetricJsonModification : IChainModification
    {
        public void Modify(BehaviorChain chain)
        {
            chain.MakeAsymmetricJson();
        }
    }

    public class SymmetricJsonModification : IChainModification
    {
        public void Modify(BehaviorChain chain)
        {
            chain.MakeSymmetricJson();
        }
    }
}