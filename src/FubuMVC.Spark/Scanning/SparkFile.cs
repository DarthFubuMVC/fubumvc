using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FubuMVC.Spark.Scanning
{
    public class SparkFile
    {
        public SparkFile(string path, string root)
        {
            Path = path;
            Root = root;
        }

        public string Path { get; private set; }
        public string Root { get; private set; }
    }

    public class SparkFiles : Collection<SparkFile>, IVisitable<SparkFile>
    {
        public void AcceptVisitor(IVisitor<SparkFile> visitor)
        {
            this.Each(visitor.Visit);
        }
    }
}