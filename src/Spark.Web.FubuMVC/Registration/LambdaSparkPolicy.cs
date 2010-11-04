using System;
using FubuMVC.Core.Registration.Nodes;

namespace Spark.Web.FubuMVC.Registration
{
    public class LambdaSparkPolicy : ISparkPolicy
    {
        private readonly Func<ActionCall, bool> _filter;
        private readonly Func<ActionCall, string> _viewLocator;
        private readonly Func<ActionCall, string> _viewName;

        public LambdaSparkPolicy(Func<ActionCall, bool> filter, Func<ActionCall, string> viewLocator, Func<ActionCall, string> viewName)
        {
            _filter = filter;
            _viewName = viewName;
            _viewLocator = viewLocator;
        }

        public bool Matches(ActionCall call)
        {
            return _filter(call);
        }

        public string BuildViewLocator(ActionCall call)
        {
            return _viewLocator(call);
        }

        public string BuildViewName(ActionCall call)
        {
            return _viewName(call);
        }
    }
}