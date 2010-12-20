namespace FubuMVC.Core.Packaging
{
    public interface IPackageManifestReader
    {
        IPackageInfo LoadFromFolder(string folder);
    }
}