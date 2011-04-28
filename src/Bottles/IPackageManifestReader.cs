namespace Bottles
{
    public interface IPackageManifestReader
    {
        IPackageInfo LoadFromFolder(string folder);
    }
}