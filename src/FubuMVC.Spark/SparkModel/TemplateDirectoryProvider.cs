using System.Collections.Generic;
using System.Linq;
using FubuMVC.Spark.SparkModel.Sharing;

namespace FubuMVC.Spark.SparkModel
{
    public interface ITemplateDirectoryProvider
    {
        IEnumerable<string> SharedPathsOf(ITemplate template);
        IEnumerable<string> ReachablesOf(ITemplate template);
    }

    public class TemplateDirectoryProvider : ITemplateDirectoryProvider
    {
        private readonly ISharedPathBuilder _builder;
        private readonly ITemplateRegistry _templates;
        private readonly ISharingGraph _graph;

        public TemplateDirectoryProvider(ISharedPathBuilder builder, ITemplateRegistry templates, ISharingGraph graph)
        {
            _builder = builder;
            _templates = templates;
            _graph = graph;
        }

        public IEnumerable<string> SharedPathsOf(ITemplate template)
        {
            return getDirectories(template, false).ToList();
        }

        public IEnumerable<string> ReachablesOf(ITemplate template)
        {
            return getDirectories(template, true).ToList();
        }

        private IEnumerable<string> getDirectories(ITemplate template, bool includeDirectAncestor)
        {
            var directories = new List<string>();

            var locals = _builder.BuildBy(template.FilePath, template.RootPath, includeDirectAncestor);            
            directories.AddRange(locals);

            _graph.SharingsFor(template.Origin).Each(sh =>
            {
                var root = _templates.ByOrigin(sh).FirstValue(t => t.RootPath);
                if (root == null) return;

                var sharings = _builder.BuildBy(root);
                directories.AddRange(sharings);                                                                                                                    
            });

            return directories;
        }
    }
}