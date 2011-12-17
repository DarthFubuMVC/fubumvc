using System;
using System.Collections.Generic;

namespace FubuMVC.Razor.RazorModel
{
    public interface IRazorDescriptor
    {
        string Name { get; }
    }

    public class ViewDescriptor : IRazorDescriptor
    {
        private readonly ITemplate _template;
        private readonly IList<ITemplate> _bindings = new List<ITemplate>();
        public ViewDescriptor(ITemplate template)
        {
            _template = template;
        }

        string IRazorDescriptor.Name { get { return "View"; } }

        public string Name() { return _template.Name(); }
        public ITemplate Master { get; set; }
        public IEnumerable<ITemplate> Bindings { get { return _bindings; } }
        public Type ViewModel { get; set; }
        public string Namespace { get; set; }
        public string ViewPath { get { return _template.ViewPath; } }
        public string RelativePath() { return _template.RelativePath(); }
        public void AddBinding(ITemplate template) { _bindings.Add(template); }

        public bool HasViewModel()
        {
            return ViewModel != null;
        }
    }

    public class NulloDescriptor : IRazorDescriptor
    {
        public string Name { get { return "Template"; } }
    }
}