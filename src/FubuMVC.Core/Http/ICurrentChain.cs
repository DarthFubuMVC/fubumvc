using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Http
{
    public interface ICurrentChain
    {
        /// <summary>
        ///   The behavior chain that is currently executing
        /// </summary>
        /// <value></value>
        BehaviorChain Current { get; }

        /// <summary>
        /// The original BehaviorChain that started this request.  This will
        /// be different than the Current property within partial requests
        /// </summary>
        BehaviorChain OriginatingChain { get; }

        IDictionary<string, object> RouteData { get; }

        // This is necessary if we wanna get handle partials too
        void Push(BehaviorChain chain);
        void Pop();

        /// <summary>
        ///   Canonical hash code unique for this resource path.  
        ///   Calculated by the chain id and the key/value pairs 
        ///   of the route
        /// </summary>
        /// <returns></returns>
        string ResourceHash();

        bool IsInPartial();
    }
}