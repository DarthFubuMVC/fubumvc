using System;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View;
using FubuMVC.Spark.Registration.Nodes;
using FubuMVC.Spark.SparkModel;
using Spark;

namespace FubuMVC.Spark
{
    public class SparkViewToken : IViewToken
    {
        private readonly ViewDescriptor _descriptor;

        public SparkViewToken(ViewDescriptor viewDescriptor)
        {
            _descriptor = viewDescriptor;
        }

        public BehaviorNode ToBehavioralNode()
        {
            return new SparkViewOutput(_descriptor);
        }

        public Type ViewType
        {
            get { return typeof(ISparkView); }
        }

        public Type ViewModelType
        {
            get { return _descriptor.ViewModel; }
        }

        public string Name
        {
            get { return _descriptor.Template.Name(); }
        }

        public string Folder
        {
            get { return _descriptor.Namespace; }
        }
        public override string ToString()
        {
            return _descriptor.Template.RelativePath();
        }
    }
}