using Bottles.Deployment.Runtime;

namespace Bottles.Deployment.Parsing
{
    public interface IProfileReader
    {
        DeploymentPlan Read(DeploymentOptions options);
    }
}