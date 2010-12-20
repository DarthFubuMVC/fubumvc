namespace FubuMVC.Core.Packaging
{
    public interface IAssemblyLoader
    {
        void ReadPackage(IPackageInfo package);
    }
}