using System;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
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
            return new SparkViewNode(_file, _call);
        }

        public Type ViewType
        {
            get { return typeof(ISparkView); }
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

    public class SparkViewNode : OutputNode<RenderSparkFubuViewBehavior>, IMayHaveInputType
    {
        private readonly SparkFile _file;
        private readonly ActionCall _call;

        public SparkViewNode(SparkFile file, ActionCall call)
        {
            _file = file;
            _call = call;
        }

        public Type InputType()
        {
            throw new NotImplementedException();
        }
    }

    public class RenderSparkFubuViewBehavior : RenderFubuViewBehavior
    {
        public RenderSparkFubuViewBehavior(SparkViewEngine<IFubuView> engine, IFubuRequest request, ViewPath view, FubuMVC.Core.View.IViewActivator activator)
            : base(engine, request, view, activator)
        {

        }
    }

    public class SparkViewEngine<T> : IViewEngine<T> where T : class
    {
        public void RenderView(ViewPath viewPath, Action<T> configureView)
        {
            throw new NotImplementedException();
        }
    }
}