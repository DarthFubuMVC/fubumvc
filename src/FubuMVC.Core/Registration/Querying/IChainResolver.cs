using System;
using System.Linq.Expressions;
using System.Reflection;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Querying
{
    public interface IChainResolver
    {
        BehaviorChain Find<T>(Expression<Action<T>> expression);

        BehaviorChain FindUnique(object model, string category = null);
        BehaviorChain Find(Type handlerType, MethodInfo method);
        BehaviorChain FindCreatorOf(Type type);

        void RootAt(string baseUrl);
        IChainForwarder FindForwarder(object model, string category = null);
        BehaviorChain FindUniqueByInputType(Type modelType, string category = null);
    }

    public enum CategorySearchMode
    {
        /// <summary>
        /// Only match if the category of the chains explicitly matches the search criteria
        /// </summary>
        Strict,

        /// <summary>
        /// If only one chain is found for all the other criteria, it's okay
        /// to ignore the exact match on Category
        /// </summary>
        Relaxed
    }

    public enum TypeSearchMode
    {
        Any,
        InputModelOnly,
        HandlerOnly
    }
}