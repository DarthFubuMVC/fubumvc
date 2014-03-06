using System;
using FubuCore;

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

        public T Template { get; set; }
        public T Master { get; set; }
        public string Namespace { get; set; }


        public Type ViewModel { get; set; }
        public bool HasViewModel()
        {
            return ViewModel != null;
        }

        public string FullName()
        {
            return Namespace.IsEmpty() ? Template.Name() : Namespace + "." + Template.Name();
        }
    }

    public class NulloDescriptor : ITemplateDescriptor
    {
        public string Name { get { return "Template"; } }
    }
}