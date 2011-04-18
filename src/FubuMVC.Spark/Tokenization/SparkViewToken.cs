using System;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View;
using FubuMVC.Spark.Registration.Nodes;
using Spark;

namespace FubuMVC.Spark.Tokenization
{
    public class SparkViewToken : IViewToken
    {
        private readonly SparkItem _item;

        public SparkViewToken(SparkItem item)
        {
            _item = item;
        }

        public BehaviorNode ToBehavioralNode()
        {
            return new SparkViewOutput(_item);
        }

        public Type ViewType
        {
            get { return typeof(ISparkView); }
        }

        public Type ViewModelType
        {
            get { return _item.ViewModelType; }
        }

        public string Name
        {
            get { return _item.Name(); }
        }

        public string Folder
        {
            get { return _item.Namespace; }
        }
        public override string ToString()
        {
            return _item.RelativePath();
        }
    }
}