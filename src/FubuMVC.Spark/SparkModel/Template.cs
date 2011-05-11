using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace FubuMVC.Spark.SparkModel
{
    public interface ITemplate
    {
        string FilePath { get; }
        string RootPath { get; }
        string Origin { get; }
		
        string ViewPath { get; set; }
        ISparkDescriptor Descriptor { get; set; }
    }

    public class Template : ITemplate
    {
        public Template(string filePath, string rootPath, string origin)
        {
            FilePath = filePath;
            RootPath = rootPath;
            Origin = origin;
            Descriptor = new NulloDescriptor();
        }

        public string FilePath { get; private set; }
        public string RootPath { get; private set; }
        public string Origin { get; private set; }
		
        public string ViewPath { get; set; }
        public ISparkDescriptor Descriptor { get; set; }

	    public override string ToString()
        {
            return FilePath;
        }
    }

    // TODO : This is a bit silly. Rework pending. 

    public class Templates : List<ITemplate>, ISparkTemplates
    {
        public Templates() {}
        public Templates(IEnumerable<ITemplate> templates) : base(templates) { }

        public IEnumerable<ITemplate> BindingsForView(string viewPath)
        {
            return this
                .Where(x => x.ViewPath == viewPath && x.Descriptor is ViewDescriptor)
                .FirstValue(x => x.Descriptor.As<ViewDescriptor>().Bindings);
        }
    }

    public interface ISparkTemplates : IEnumerable<ITemplate>
    {
        IEnumerable<ITemplate> BindingsForView(string viewPath);
    }
}