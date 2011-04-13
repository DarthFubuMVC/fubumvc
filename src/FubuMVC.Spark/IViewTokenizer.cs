using System.Collections.Generic;
using System.Linq;
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
        private readonly IEnumerable<ISparkMatcher> _matchers;
        private readonly IEnumerable<SparkFile> _sparkFiles;

        public ViewTokenizer(IEnumerable<ISparkMatcher> matchers, IEnumerable<SparkFile> sparkFiles)
        {
            _matchers = matchers;
            _sparkFiles = sparkFiles;
        }

        public IEnumerable<SparkViewToken> Tokenize(IEnumerable<ActionCall> actionCalls)
        {
            foreach (var actionCall in actionCalls)
            {
                var call = actionCall;
                foreach (var file in _sparkFiles.Where(file => _matchers.Any(m => m.Match(file, call))))
                {
                    yield return new SparkViewToken(file);
                }
            }
        }
    }

    public interface ISparkMatcher
    {
        bool Match(SparkFile file, ActionCall call);
    }
}