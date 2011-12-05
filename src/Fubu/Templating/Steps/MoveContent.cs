using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;

namespace Fubu.Templating.Steps
{
    public class MoveContent : ITemplateStep
    {
        public static readonly string FubuIgnoreFile = ".fubuignore";

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
            var fileSet = new FileSet
                                   {
                                       DeepSearch = false,
                                       Include = "*.*",
                                       Exclude = "*.exe;*.dll;.git;{0};{1};".ToFormat(FubuIgnoreFile, AutoRunFubuRake.FubuRakeFile)
                                   };
            var fubuIgnore = FileSystem.Combine(context.TempDir, FubuIgnoreFile);
            if(_fileSystem.FileExists(fubuIgnore))
            {
                _fileSystem
                    .ReadStringFromFile(fubuIgnore)
                    .SplitOnNewLine()
                    .Each(ignore =>
                              {
                                  fileSet.Exclude += "{0};".ToFormat(ignore);
                              });
            }

            var excludedFiles = fileSet.ExcludedFilesFor(context.TempDir);
            _fileSystem
                .FindFiles(context.TempDir, fileSet)
                .Where(file => !excludedFiles.Contains(file))
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
        }
    }
}