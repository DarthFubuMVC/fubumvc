using FubuMVC.Core.Registration.Nodes;

namespace Spark.Web.FubuMVC.Registration
{
    public interface ISparkPolicy
    {
        bool Matches(ActionCall callz);
        string BuildViewLocator(ActionCall call);
        string BuildViewName(ActionCall call);
    }
}