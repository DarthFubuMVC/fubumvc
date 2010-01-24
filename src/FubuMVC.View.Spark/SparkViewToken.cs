using System;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View;

namespace FubuMVC.View.Spark
{
    public class SparkViewToken : IViewToken
    {
        private readonly string _viewPath;

        public SparkViewToken(string viewPath)
        {
            _viewPath = viewPath;
        }

        public BehaviorNode ToBehavioralNode()
        {
            return new SparkViewNode(_viewPath);
        }

        public Type ViewType
        {
            get { return null; } }

        public Type ViewModelType
        {
            get { return null; }
        }

        // TODO -- flesh this out
        public string Name
        {
            get { return null; }
        }

        // TODO -- flesh this out
        public string Folder
        {
            get { return null; }
        }

        public override string ToString()
        {
            return _viewPath;
        }
    }
}