using Bottles.Diagnostics;

namespace Bottles.Environment
{
    public interface IEnvironmentRequirement
    {
        string Describe();
        void Check(IPackageLog log);
    }

}