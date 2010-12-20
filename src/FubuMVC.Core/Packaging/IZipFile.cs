namespace FubuMVC.Core.Packaging
{
    public interface IZipFile
    {
        void AddFile(string fileName);
        void AddFile(string fileName, string zipFolder);

        void AddFiles(ZipFolderRequest request);
    }
}