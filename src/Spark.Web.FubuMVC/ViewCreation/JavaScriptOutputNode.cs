using System;
using Spark.Web.FubuMVC.ViewCreation;
using FubuMVC.Core.Registration.Nodes;

namespace Spark.Web.FubuMVC.ViewCreation
{
    public class JavaScriptOutputNode : SparkViewNode
    {
        public JavaScriptOutputNode(SparkViewToken viewToken, ActionCall actionCall)
            : base(viewToken, actionCall) { }

        public override string Description
        {
            get
            {
                return String.Format("Spark View '{0}' as JavaScript", _viewToken.Name);
            }
        }
    }
}
