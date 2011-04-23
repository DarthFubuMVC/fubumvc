using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Spark.SparkModel
{
    public class SparkItem
    {
        public SparkItem(string filePath, string rootPath, string origin)
        {
            FilePath = filePath;
            RootPath = rootPath;
            Origin = origin;
        }

        public string FilePath { get; private set; }
        public string RootPath { get; private set; }
        public string Origin { get; private set; }

        public SparkItem Master { get; set; }
        public Type ViewModelType { get; set; }
        
        public string Namespace { get; set; }
        public string ViewPath { get; set; }

        public override string ToString()
        {
            return FilePath;
        }
    }

    // We need to get this populated once and then pass around an interface.
    // Or kill it.
    public class SparkItems : List<SparkItem> // TODO: Create lookup registry interface from this
    {
        public SparkItems(){}
        public SparkItems(IEnumerable<SparkItem> items) : base(items) {}

        // Temporary : probably ends up as extension method on IEnumerable...

        public IEnumerable<SparkItem> ByName(string name)
        {
            return this.Where(x => x.Name() == name);
        }

        public SparkItem FirstByName(string name)
        {
            return ByName(name).FirstOrDefault();
        }
    }
}