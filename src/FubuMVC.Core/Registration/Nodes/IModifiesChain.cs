namespace FubuMVC.Core.Registration.Nodes
{
    public interface IModifiesChain
    {
        void Modify(BehaviorChain chain);
    }
}