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

    public interface ICurrentRequest
    {
        /// <summary>
        /// Full url of the request, never contains a trailing /
        /// </summary>
        /// <returns></returns>
        string RawUrl();

        /// <summary>
        /// Url relative to the application
        /// </summary>
        /// <returns></returns>
        string RelativeUrl();

        /// <summary>
        /// Base root of the application
        /// </summary>
        /// <returns></returns>
        string ApplicationRoot();

        string HttpMethod();


    }
}