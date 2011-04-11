using System;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View;
using FubuMVC.Spark.Scanning;
using Spark;

namespace FubuMVC.Spark
{
    public class SparkViewToken : IViewToken
    {
        private readonly SparkFile _file;
        private readonly ActionCall _call;

        public SparkViewToken(SparkFile file, ActionCall call)
        {
            _file = file;
            _call = call;
        }

        public BehaviorNode ToBehavioralNode()
        {
            throw new NotImplementedException();
        }

        public Type ViewType
        {
            get { return typeof (ISparkView); }
        }

        public Type ViewModelType
        {
            get { return _call.OutputType(); }
        }

        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        public string Folder
        {
            get { throw new NotImplementedException(); }
        }
    }
}