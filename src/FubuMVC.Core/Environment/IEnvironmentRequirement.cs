using FubuMVC.Core.Diagnostics.Packaging;

namespace FubuMVC.Core.Environment
{
    public interface IEnvironmentRequirement
    {
        string Describe();
        void Check(IPackageLog log);
    }

}