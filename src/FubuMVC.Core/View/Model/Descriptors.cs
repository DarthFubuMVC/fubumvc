using System;
using FubuCore;

namespace FubuMVC.Core.View.Model
{
    public interface ITemplateDescriptor
    {
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

        public T Template { get; set; }
        public T Master { get; set; }

    }

    public class NulloDescriptor : ITemplateDescriptor
    {
        public string Name { get { return "Template"; } }
    }
}