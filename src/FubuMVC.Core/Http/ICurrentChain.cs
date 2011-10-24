using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Http
{
    public interface ICurrentChain
    {
        /// <summary>
        /// The behavior chain that is currently executing
        /// </summary>
        /// <returns></returns>
        BehaviorChain CurrentChain();

        // This is necessary if we wanna get handle partials too
        void Push(BehaviorChain chain);
        void Pop();
    }
}