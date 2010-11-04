using FubuMVC.Core.Registration.Nodes;

namespace Spark.Web.FubuMVC.Registration
{
    public interface ISparkPolicy
    {
        bool Matches(ActionCall call);
        string BuildViewLocator(ActionCall call);
        string BuildViewName(ActionCall call);
    }
}