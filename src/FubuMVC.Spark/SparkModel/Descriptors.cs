using System;
using System.Collections.Generic;

namespace FubuMVC.Spark.SparkModel
{
    public interface ISparkDescriptor
    {
        string Name { get; }
    }

    // TODO: UT
    public class ViewDescriptor : ISparkDescriptor
    {
        private readonly ITemplate _template;
        private readonly IList<ITemplate> _bindings = new List<ITemplate>();
        public ViewDescriptor(ITemplate template)
        {
            _template = template;
        }

        string ISparkDescriptor.Name { get { return "View"; } }

        public string Name() { return _template.Name(); }

        public ITemplate Master { get; set; }
        public string Namespace { get; set; }

        public string ViewPath { get { return _template.ViewPath; } }
        public string RelativePath() { return _template.RelativePath(); }

        public void AddBinding(ITemplate template) { _bindings.Add(template); }
        public IEnumerable<ITemplate> Bindings { get { return _bindings; } }

        public Type ViewModel { get; set; }
        public bool HasViewModel()
        {
            return ViewModel != null;
        }
    }

    public class NulloDescriptor : ISparkDescriptor
    {
        public string Name { get { return "Template"; } }
    }
}