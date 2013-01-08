using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Packaging;

namespace FubuMVC.Core.Runtime.Files
{
    public class FubuApplicationFiles : IFubuApplicationFiles
    {
        private readonly Cache<string, IFubuFile> _files;

        private readonly Lazy<IEnumerable<ContentFolder>> _folders
            = new Lazy<IEnumerable<ContentFolder>>(ContentFolder.FindAllContentFolders);

        public FubuApplicationFiles()
        {
            _files = new Cache<string, IFubuFile>(findFile);
        }

        public string GetApplicationPath()
        {
            return FubuMvcPackageFacility.GetApplicationPath();
        }

        // I'm okay with this finding nulls

        public IEnumerable<IFubuFile> FindFiles(FileSet fileSet)
        {
            fileSet.AppendExclude(FubuMvcPackageFacility.FubuContentFolder + "/*.*");
            fileSet.AppendExclude(FubuMvcPackageFacility.FubuPackagesFolder + "/*.*");

            return _folders.Value.SelectMany(folder => folder.FindFiles(fileSet))
                .Where(IsNotUnderExplodedBottleFolder);
        }

        public static bool IsNotUnderExplodedBottleFolder(IFubuFile fubuFile)
        {
            return !fubuFile.RelativePath.Contains(FubuMvcPackageFacility.FubuContentFolder) 
                && !fubuFile.RelativePath.Contains(FubuMvcPackageFacility.FubuPackagesFolder);
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
                var files = AllFolders.SelectMany(x => FindFiles(FileSet.Deep("*"))).Select(x => x.Path);

                var description = "Could not find " + relativeName;
                files.Each(x => description += "\n" + x);

                throw new ApplicationException(description);
            }
        }

        public IEnumerable<ContentFolder> AllFolders
        {
            get { return _folders.Value; }
        }

        private IFubuFile findFile(string name)
        {
            var fileSet = new FileSet{
                DeepSearch = true,
                Include = name
            };

            return FindFiles(fileSet).FirstOrDefault();
        }
    }
}