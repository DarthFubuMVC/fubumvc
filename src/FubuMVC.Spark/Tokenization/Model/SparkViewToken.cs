using System;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View;
using Spark;

namespace FubuMVC.Spark.Tokenization.Model
{
    public class SparkViewToken : IViewToken
    {
        private readonly SparkFile _file;

        public SparkViewToken(SparkFile file)
        {
            _file = file;
        }

        public BehaviorNode ToBehavioralNode()
        {
            return null;
        }

        public Type ViewType
        {
            get { return typeof(ISparkView); }
        }

        public Type ViewModelType
        {
            get { return _file.ViewModel; }
        }

        public string Name
        {
            get { return _file.Name(); }
        }

        public string Folder
        {
            get { return _file.Namespace; }
        }
        public override string ToString()
        {
            return _file.RelativePath();
        }
    }
}