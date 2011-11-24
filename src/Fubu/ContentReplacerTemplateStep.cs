using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;

namespace Fubu
{
    public class ContentReplacerTemplateStep : ITemplateStep
    {
        private static readonly FileSet TopLevelFileSet = new FileSet
                                                              {
                                                                  DeepSearch = false,
                                                                  Include = "*.*",
                                                                  Exclude = "*.exe;*.dll"
                                                              };

        private readonly IKeywordReplacer _keywordReplacer;
        private readonly IFileSystem _fileSystem;

        public ContentReplacerTemplateStep(IKeywordReplacer keywordReplacer, IFileSystem fileSystem)
        {
            _keywordReplacer = keywordReplacer;
            _fileSystem = fileSystem;
        }

        public void Execute(TemplatePlanContext context)
        {
            _keywordReplacer.SetToken("FUBUPROJECTNAME", context.Input.ProjectName);
            _keywordReplacer.SetToken("FUBUPROJECTSHORTNAME", context.Input.ProjectName.Split('.').Last());
            _keywordReplacer.SetToken("FACEFACE-FACE-FACE-FACE-FACEFACEFACE", Guid.NewGuid().ToString().ToUpper());

            ParseDirectory(context.TargetPath);
        }

        public void ParseDirectory(string directory)
        {
            var newDirectoryName = _keywordReplacer.Replace(directory);

            // Need to handle if there is an existing directory here.
            if (directory != newDirectoryName)
            {
                Console.WriteLine("{0} -> {1}", directory, _fileSystem.GetFileName(newDirectoryName));
                if (_fileSystem.DirectoryExists(newDirectoryName))
                {
                    _fileSystem.DeleteDirectory(newDirectoryName);
                }
                _fileSystem.MoveDirectory(directory, newDirectoryName);
                directory = newDirectoryName;
            }

            _fileSystem.FindFiles(directory, TopLevelFileSet).Each(ParseFile);
            _fileSystem.ChildDirectoriesFor(directory).Each(ParseDirectory);
        }

        public void ParseFile(string file)
        {
            var fileContent = _fileSystem.ReadStringFromFile(file);

            var replacedFileContent = _keywordReplacer.Replace(fileContent);
            if (fileContent != replacedFileContent)
            {
                _fileSystem.WriteStringToFile(file, replacedFileContent);
            }

            var newFileName = _keywordReplacer.Replace(file);
            if (file != newFileName)
            {
                Console.WriteLine("{0} -> {1}", file, Path.GetFileName(newFileName));
                if (_fileSystem.FileExists(newFileName))
                {
                    _fileSystem.DeleteFile(newFileName);
                }
                _fileSystem.MoveFile(file, newFileName);
            }
        }
    }
}