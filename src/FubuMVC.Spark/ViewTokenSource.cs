using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Spark
{
    public interface IViewTokenSource
    {
        IEnumerable<SparkViewToken> FindFrom(IEnumerable<ActionCall> actionCalls);
    }

    public class ViewTokenSource : IViewTokenSource
    {
        public ViewTokenSource(/* Injected builder and matchers */)
        {
            
        }

        public IEnumerable<SparkViewToken> FindFrom(IEnumerable<ActionCall> actionCalls)
        {
            throw new NotImplementedException();
        }
    }
}