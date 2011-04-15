using System;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime;
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
            return new SparkViewOutput(_file);
        }

        public Type ViewType
        {
            get { return typeof(ISparkView); }
        }

        public Type ViewModelType
        {
            get { return _file.ViewModelType; }
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

    //NOTE:TEMP
    public class SparkViewOutput : OutputNode<SparkViewRenderer>
    {
        private readonly SparkFile _file;

        public SparkViewOutput(SparkFile file)
        {
            _file = file;
        }
        protected override void configureObject(ObjectDef def)
        {
            def.DependencyByValue(_file);
        }
        public override string Description
        {
            get
            {
                return string.Format("Spark View [{0}]", _file.RelativePath());
            }
        }
    }

    //NOTE:TEMP
    public class SparkViewRenderer : BasicBehavior
    {
        private readonly IOutputWriter _writer;
        private readonly SparkFile _file;

        public SparkViewRenderer(IOutputWriter writer,SparkFile file)
            : base(PartialBehavior.Executes)
        {
            _writer = writer;
            _file = file;
        }

        protected override DoNext performInvoke()
        {
            _writer.WriteHtml(_file);
            return DoNext.Continue;
        }
    }
}