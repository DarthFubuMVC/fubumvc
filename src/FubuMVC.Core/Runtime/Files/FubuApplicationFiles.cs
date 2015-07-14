using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Util;

namespace FubuMVC.Core.Runtime.Files
{
    public class FubuApplicationFiles : IFubuApplicationFiles
    {
        private readonly Cache<string, IFubuFile> _files;
        private readonly static IFileSystem _fileSystem = new FileSystem();

        public FubuApplicationFiles()
        {
            _files = new Cache<string, IFubuFile>(findFile);
        }

        public string GetApplicationPath()
        {
            return FubuApplication.GetApplicationPath();
        }

        // I'm okay with this finding nulls

        public IEnumerable<IFubuFile> FindFiles(FileSet fileSet)
        {
            var applicationPath = GetApplicationPath();
            return _fileSystem.FindFiles(applicationPath, fileSet).Select(file =>
            {
                var fubuFile = new FubuFile(file)
                {
                    RelativePath = file.PathRelativeTo(applicationPath).Replace("\\", "/")
                };

                if (fubuFile.RelativePath.IsEmpty())
                {
                    throw new ArgumentException("Not able to determine a relative path for " + file);
                }

                return fubuFile;
            });
        }


        public IFubuFile Find(string relativeName)
        {
            return _files[relativeName.Replace("\\", "/")];
        }

        public void AssertHasFile(string relativeName)
        {
            relativeName = relativeName.Replace("\\", "/");
            var file = findFile(relativeName);
            if (file == null)
            {
                var files = FindFiles(FileSet.Deep("*")).Select(x => x.Path);

                var description = "Could not find " + relativeName;
                files.Each(x => description += "\n" + x);

                throw new ApplicationException(description);
            }
        }

        private IFubuFile findFile(string name)
        {
            if (name.IsEmpty()) return null;

            var fileSet = new FileSet{
                DeepSearch = true,
                Include = name
            };

            return FindFiles(fileSet).FirstOrDefault();
        }
    }
}