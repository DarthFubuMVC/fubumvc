using System.Collections.Generic;
using FubuMVC.Core.Registration;
using FubuMVC.Spark.Tokenization.Model;

namespace FubuMVC.Spark.Tokenization
{
    public interface IViewTokenizer
    {
        IEnumerable<SparkViewToken> Tokenize(TypePool types, BehaviorGraph graph);
    }
}