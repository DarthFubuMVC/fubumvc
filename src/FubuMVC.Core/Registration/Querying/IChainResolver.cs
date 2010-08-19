using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Querying
{
    public interface IChainResolver
    {
        BehaviorChain Find<T>(Expression<Action<T>> expression);

        IEnumerable<BehaviorChain> Find(object model);
        BehaviorChain FindUnique(object model);
        BehaviorChain FindUnique(object model, string category);
    }
}