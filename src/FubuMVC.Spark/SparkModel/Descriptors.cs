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
        private readonly IList<ITemplate> _bindings = new List<ITemplate>();

        public ViewDescriptor(ITemplate template)
        {
            Template = template;
            // Ahh..
            Template.Descriptor = this;
        }

        public string Name { get { return "View"; } }

        public ITemplate Template { get; private set; }
        public ITemplate Master { get; set; }
        public IEnumerable<ITemplate> Bindings
        {
            get { return _bindings; }
        }

        public Type ViewModel { get; set; }
        public string Namespace { get; set; }

        public void AddBinding(ITemplate template)
        {
            _bindings.Add(template);
        }

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