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
    }
}