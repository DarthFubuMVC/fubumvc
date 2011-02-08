using System.Collections.Generic;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Packaging;

namespace FubuMVC.Core.Content
{
    public interface IContentFolderService
    {
        string FileNameFor(ContentType contentType, string contentFileName);
        bool FileExists(ContentType contentType, string contentFileName);
        void RegisterDirectory(string directory);

        bool ExistsInApplicationDirectory(ContentType contentType, string contentFileName);
    }


    public enum ContentType
    {
        images,
        scripts,
        styles
    }

    public class ContentFolderService : IContentFolderService
    {
        private readonly IList<string> _directories = new List<string>();
        private readonly Cache<string, string> _fileNames = new Cache<string, string>();
        private readonly IFileSystem _fileSystem;

        public ContentFolderService(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;

            _fileNames.OnMissing =
                name =>
                {
                    return
                        _directories.FirstValue(
                            dir => { return _fileSystem.FileExists(dir, name) ? FileSystem.Combine(dir, name) : null; });
                };
        }

        public void RegisterDirectory(string directory)
        {
            _directories.Add(directory);
        }

        public bool ExistsInApplicationDirectory(ContentType contentType, string contentFileName)
        {
            return _fileSystem.FileExists(FubuMvcPackageFacility.GetApplicationPath(), "content", contentType.ToString(),
                                          contentFileName);
        }

        public string FileNameFor(ContentType contentType, string contentFileName)
        {
            return _fileNames[FileSystem.Combine(contentType.ToString(), contentFileName)];
        }

        public bool FileExists(ContentType contentType, string contentFileName)
        {
            return FileNameFor(contentType, contentFileName) != null;
        }
    }
}