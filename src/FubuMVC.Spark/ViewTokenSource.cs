using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Spark.Scanning;

namespace FubuMVC.Spark
{
    public interface IViewTokenSource
    {
        IEnumerable<SparkViewToken> FindFrom(IEnumerable<ActionCall> actionCalls);
    }

    public class ViewTokenSource : IViewTokenSource
    {
        private readonly IEnumerable<IViewTokenStrategy> _strategies;

        public ViewTokenSource(IEnumerable<IViewTokenStrategy> strategies)
        {
            _strategies = strategies;
        }

        public IEnumerable<SparkViewToken> FindFrom(IEnumerable<ActionCall> actionCalls)
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
            _cache = new Cache<ActionCall, SparkFile>();
            _files = files;
        }

        public bool Applies(ActionCall call)
        {
            var ns = call.HandlerType.Namespace;
            var methodName = call.Method.Name;
            foreach (var file in _files.Where(file => ns == file.Ns() && methodName == file.Name()))
            {
                _cache.Fill(call, file);
                return true;
            }
            return false;
        }


        public SparkFile Get(ActionCall call)
        {
            return _cache[call];
        }
    }
}