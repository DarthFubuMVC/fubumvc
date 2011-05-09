using System.Collections.Generic;

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

    public class SparkItem : ITemplate
    {
        public SparkItem(string filePath, string rootPath, string origin)
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

    public class SparkTemplates : List<ITemplate>, ISparkTemplates
    {
        public SparkTemplates() {}
        public SparkTemplates(IEnumerable<ITemplate> templates) : base(templates) { }
    }

    public interface ISparkTemplates : IEnumerable<ITemplate> { }
}