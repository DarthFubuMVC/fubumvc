﻿using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.View.Model.Sharing;

namespace FubuMVC.Core.View.Model
{
    public interface ITemplateDirectoryProvider<T> where T : ITemplateFile
    {
        IEnumerable<string> SharedPathsOf(T template);
        IEnumerable<string> ReachablesOf(T template);
    }

    public class TemplateDirectoryProvider<T> : ITemplateDirectoryProvider<T> where T : ITemplateFile
    {
        private readonly ISharedPathBuilder _builder;
        private readonly ITemplateRegistry<T> _templates;
        private readonly ISharingGraph _graph;

        public TemplateDirectoryProvider(ISharedPathBuilder builder, ITemplateRegistry<T> templates, ISharingGraph graph)
        {
            _builder = builder;
            _templates = templates;
            _graph = graph;
        }

        public IEnumerable<string> SharedPathsOf(T template)
        {
            return getDirectories(template, false).ToList();
        }

        public IEnumerable<string> ReachablesOf(T template)
        {
            return getDirectories(template, true).ToList();
        }

        private IEnumerable<string> getDirectories(T template, bool includeDirectAncestor)
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