using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Configuration.Partials;
using Spark.Web.FubuMVC.Extensions;
using Spark.Web.FubuMVC.Registration;

namespace FubuMVC.Diagnostics.Configuration.Spark
{
    public class PartialActionSparkPolicy : ISparkPolicy
    {
        public bool Matches(ActionCall call)
        {
            return call.HandlerType.Closes(typeof (PartialAction<>));
        }

        public string BuildViewLocator(ActionCall call)
        {
            return "Shared";
        }

        public string BuildViewName(ActionCall call)
        {
            return call.InputType().Name.RemoveSuffix("Model");
        }
    }
}