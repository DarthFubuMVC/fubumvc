using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View;

namespace Spark.Web.FubuMVC.ViewCreation
{
    public class SparkViewToken : IViewToken
    {
        private readonly ActionCall _actionCall;
        private readonly SparkViewDescriptor _matchedDescriptor;
        private readonly string _actionName;
        private readonly IList<SparkViewDescriptor> _descriptors;

        public SparkViewToken(ActionCall actionCall, SparkViewDescriptor matchedDescriptor, string actionName)
        {
            _actionCall = actionCall;
            _matchedDescriptor = matchedDescriptor;
            _actionName = actionName;
        }

        public SparkViewToken(IList<SparkViewDescriptor> descriptors)
        {
            _descriptors = descriptors;
        }

        public IList<SparkViewDescriptor> Descriptors
        {
            get { return _descriptors; }
        }

        public string ActionName
        {
            get { return _actionName; }
        }

        #region IViewToken Members

        public BehaviorNode ToBehavioralNode()
        {
            return new SparkViewNode(this, _actionCall);
        }

        public Type ViewType
        {
            get { return typeof (ISparkView); }
        }

        public Type ViewModelType
        {
            get { return _actionCall.InputType(); }
        }

        public string Name
        {
            get { return _actionCall.Method.Name; }
        }

        public string Folder
        {
            get { return null; }
        }

        #endregion
    }
}