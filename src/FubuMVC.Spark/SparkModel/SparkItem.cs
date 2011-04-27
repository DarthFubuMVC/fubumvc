using System;
using System.Collections;
using System.Collections.Generic;

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

    public class SparkItems : List<SparkItem>, ISparkItems
    {
        public SparkItems() {}
        public SparkItems(IEnumerable<SparkItem> items) : base(items) {}
    }

    public interface ISparkItems : IEnumerable<SparkItem> {}
}