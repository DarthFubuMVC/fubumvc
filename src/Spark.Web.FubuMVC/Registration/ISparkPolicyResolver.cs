using FubuMVC.Core.Registration.Nodes;

namespace Spark.Web.FubuMVC.Registration
{
    public interface ISparkPolicyResolver
    {
        string ResolveViewLocator(ActionCall call);
        string ResolveViewName(ActionCall call);
    }
}