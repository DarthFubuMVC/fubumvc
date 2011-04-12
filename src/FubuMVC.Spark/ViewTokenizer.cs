using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Spark.Scanning;

namespace FubuMVC.Spark
{
    public interface IViewTokenizer
    {
        IEnumerable<SparkViewToken> Tokenize(IEnumerable<ActionCall> actionCalls);
    }

    public class ViewTokenizer : IViewTokenizer
    {
        private readonly IEnumerable<IViewTokenStrategy> _strategies;

        public ViewTokenizer(IEnumerable<IViewTokenStrategy> strategies)
        {
            _strategies = strategies;
        }

        public IEnumerable<SparkViewToken> Tokenize(IEnumerable<ActionCall> actionCalls)
        {
            foreach (var call in actionCalls)
            {
                var local = call;
                var strategy = _strategies.FirstOrDefault(x => x.Applies(local));
                if (strategy == null)
                {
                    continue;
                }
                var file = strategy.Get(local);
                yield return new SparkViewToken(file, local);
            }
        }
    }

    public interface IViewTokenStrategy
    {
        bool Applies(ActionCall call);
        SparkFile Get(ActionCall call);
    }

    public class ByNamespaceStrategy : IViewTokenStrategy
    {
        private readonly IEnumerable<SparkFile> _files;
        private readonly Cache<ActionCall, SparkFile> _cache;

        public ByNamespaceStrategy(IEnumerable<SparkFile> files)
        {
            _cache = new Cache<ActionCall, SparkFile>(locator);
            _files = files;
        }

        private SparkFile locator(ActionCall actionCall)
        {
            return _files.FirstOrDefault(f => 
                actionCall.Method.Name == f.Name() && 
                actionCall.HandlerType.Namespace == f.Namespace());
        }

        public bool Applies(ActionCall call)
        {
            return _cache[call] != null;
        }

        public SparkFile Get(ActionCall call)
        {
            return _cache[call];
        }
    }
}