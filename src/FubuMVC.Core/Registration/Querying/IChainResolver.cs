using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Querying
{
    public interface IChainResolver
    {
        BehaviorChain Find<T>(Expression<Action<T>> expression);

        IEnumerable<BehaviorChain> Find(object model);
        BehaviorChain FindUnique(object model);
        BehaviorChain FindUnique(object model, string category);
        BehaviorChain Find(Type handlerType, MethodInfo method);
        BehaviorChain FindCreatorOf(Type type);
        void RootAt(string baseUrl);
        IChainForwarder FindForwarder(object model, string category);
        IChainForwarder FindForwarder(object model);
        BehaviorChain FindUniqueByInputType(Type modelType);
        BehaviorChain FindUniqueByInputType(Type modelType, string category);
    }
}