using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
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
            info
                .EnumerateDirectories(".git")
                .First()
                .EnumerateFiles("*", SearchOption.AllDirectories)
                .Each(fileInfo =>
                          {
                              fileInfo.IsReadOnly = false;
                          });
            info.Attributes &= ~FileAttributes.ReadOnly;

            _fileSystem.DeleteDirectory(context.TempDir);
        }
    }
}