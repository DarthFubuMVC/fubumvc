using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Spark
{
    public interface IViewTokenizer
    {
        IEnumerable<SparkViewToken> Tokenize(IEnumerable<ActionCall> actionCalls);
    }
}