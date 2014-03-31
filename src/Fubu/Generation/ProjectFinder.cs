using System;
using FubuCore;
using FubuCsProjFile;
using System.Linq;
using System.Collections.Generic;

namespace Fubu.Generation
{
    public static class ProjectFinder
    {
        private static readonly IFileSystem fileSystem = new FileSystem();

         public static Location DetermineLocation(string path)
         {
             var directory = path.ToFullPath();
             var file = findFile(directory);
             while (file == null)
             {
                 directory = directory.ParentDirectory();
                 file = findFile(directory);
             }

             if (file == null)
             {
                 throw new Exception("Could not determine a csproj file");
             }

             var relativePath = path.ToFullPath().PathRelativeTo(file.ProjectDirectory.ToFullPath()).Replace("/", "\\");
             var @namespace = relativePath.IsNotEmpty() ? file.AssemblyName + "." + relativePath.Replace("\\", ".") : file.AssemblyName;
             return new Location
             {
                 Namespace = @namespace,
                 Project = file,
                 RelativePath = relativePath,
                 CurrentFolder = path
             };
         }

        private static CsProjFile findFile(string directory)
        {
            var files = fileSystem.FindFiles(directory, FileSet.Shallow("*.csproj"));
            var count = files.Count();
            switch (count)
            {
                case 0:
                    return null;

                case 1:
                    return CsProjFile.LoadFrom(files.Single());
            }

            throw new Exception("Cannot determine a specific CsProjFile.  Candidates include " + files.Join(", "));
        }
    }

    public class Location
    {
        public CsProjFile Project { get; set; }
        public string Namespace { get; set; }
        public string CurrentFolder { get; set; }
        public string RelativePath { get; set; }

        public string ProjectFolder()
        {
            return Project.FileName.ParentDirectory();
        }

        public string SrcFolder()
        {
            return ProjectFolder().ParentDirectory();
        }
    }
}