using System;
using System.IO;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View;

namespace FubuMVC.View.Spark
{
    public class SparkViewToken : IViewToken
    {
        private readonly string _viewPath;
        private readonly string _name;

        public SparkViewToken(string viewPath)
        {
            _viewPath = viewPath;
            _name = Path.GetFileNameWithoutExtension(_viewPath);
            Namespace = Path.GetDirectoryName(_viewPath).Replace('/', '.').Replace('\\', '.');
        }

        public BehaviorNode ToBehavioralNode()
        {
            return new SparkViewNode(Name);
        }

        public Type ViewModelType
        {
            get { return GetType(); } // intentionally prevent this from ever matching
        }

        public string Namespace { get; private set; }

        public string Name
        {
            get { return _name; }
        }
    }
}