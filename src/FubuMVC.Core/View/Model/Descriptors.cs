using System;
using System.Collections.Generic;

namespace FubuMVC.Core.View.Model
{
    public interface ITemplateDescriptor
    {
        string Name { get; }
    }

    public interface ITemplateDescriptor<T> : ITemplateDescriptor where T : ITemplateFile
    {
        T Template { get; }
    }

    // TODO: UT
    public class ViewDescriptor<T> : ITemplateDescriptor<T> where T : ITemplateFile
    {
        public ViewDescriptor(T template)
        {
            Template = template;
        }

        string ITemplateDescriptor.Name { get { return "View"; } }

        public string Name() { return Template.Name(); }

        public T Template { get; set; }
        public T Master { get; set; }
        public string Namespace { get; set; }

        public string ViewPath { get { return Template.ViewPath; } }
        public string RelativePath() { return Template.RelativePath(); }


        public Type ViewModel { get; set; }
        public bool HasViewModel()
        {
            return ViewModel != null;
        }
    }

    public class NulloDescriptor : ITemplateDescriptor
    {
        public string Name { get { return "Template"; } }
    }
}