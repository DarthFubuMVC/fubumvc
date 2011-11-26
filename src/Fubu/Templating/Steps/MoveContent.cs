using System.Collections.Generic;
using System.IO;
using FubuCore;

namespace Fubu.Templating.Steps
{
    public class MoveContent : ITemplateStep
    {
        private static readonly FileSet TopLevelFileSet = new FileSet
                                                              {
                                                                  DeepSearch = false,
                                                                  Include = "*.*",
                                                                  Exclude = "*.exe;*.dll,.git"
                                                              };

        private readonly IFileSystem _fileSystem;

        public MoveContent(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public string Describe(TemplatePlanContext context)
        {
            return "Move templated content to {0}".ToFormat(context.TargetPath);
        }

        public void Execute(TemplatePlanContext context)
        {
            _fileSystem
                .FindFiles(context.TempDir, TopLevelFileSet)
                .Each(from =>
                          {
                              var destination = Path.Combine(context.TargetPath, _fileSystem.GetFileName(from));
                              if (_fileSystem.FileExists(destination)) _fileSystem.DeleteFile(destination);
                              _fileSystem.MoveFile(from, destination);
                          });

            _fileSystem
                .ChildDirectoriesFor(context.TempDir)
                .Each(directory =>
                          {
                              var destinationName = _fileSystem.GetFileName(directory);
                              if(destinationName == ".git")
                              {
                                  return;
                              }

                              var destination = Path.Combine(context.TargetPath, destinationName);
                              if (_fileSystem.DirectoryExists(destination))
                              {
                                  _fileSystem.DeleteDirectory(destination);
                              }
                              _fileSystem.MoveDirectory(directory, destination);
                          });

            var info = new DirectoryInfo(context.TempDir);
            info.SafeDelete();
        }
    }

    public static class DirectoryExtensions
    {
        public static void SafeDelete(this FileSystemInfo info)
        {
            info.Attributes &= ~FileAttributes.ReadOnly;
            var d = info as DirectoryInfo;
            if (d != null)
            {
                var files = d.GetFileSystemInfos("*", SearchOption.TopDirectoryOnly);
                foreach(var f in files)
                {
                    f.SafeDelete();
                }
            }

            info.Delete();
        }
    }
}