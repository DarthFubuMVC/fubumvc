using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View;
using FubuMVC.Spark.Tokenization;

namespace FubuMVC.Spark
{
    public class SparkViewFacility : IViewFacility
    {
        private readonly IViewTokenizer _tokenizer;
        public SparkViewFacility(IViewTokenizer tokenizer)
        {
            _tokenizer = tokenizer;
        }

        public IEnumerable<IViewToken> FindViews(TypePool types, BehaviorGraph graph)
        {
            return _tokenizer.Tokenize(types, graph);
        }

        public BehaviorNode CreateViewNode(Type type)
        {
            throw new NotImplementedException();
        }
    }
}