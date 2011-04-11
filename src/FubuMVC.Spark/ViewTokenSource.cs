using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                if(strategy!=null)
                {
                    var result = strategy.Get(call);
                    yield return result;
                }
            }
        }
    }
    public interface IViewTokenStrategy
    {
        bool Applies(ActionCall call);
        SparkViewToken Get(ActionCall call);
    }


    public class ByNamespaceStrategy : IViewTokenStrategy
    {
        private readonly IEnumerable<SparkFile> _files;
        private SparkFile _found;
        public ByNamespaceStrategy(IEnumerable<SparkFile> files)
        {
            _files = files;
        }

        public bool Applies(ActionCall call)
        {
            var methodNamespace = string.Format("{0}{1}{2}.spark", call.HandlerType.Namespace.Split('.').Join(Path.DirectorySeparatorChar.ToString()),
                                                Path.DirectorySeparatorChar, call.Method.Name);
            foreach (var file in _files)
            {
                var fileVirtualPath = file.Path.Replace(file.Root, "").TrimStart(Path.AltDirectorySeparatorChar);
                if (fileVirtualPath != methodNamespace)
                {
                    continue;
                }
                _found = file;
                return true;
            }
            return false;
        }


        public SparkViewToken Get(ActionCall call)
        {
            return new SparkViewToken(_found, call);
        }
    }
}