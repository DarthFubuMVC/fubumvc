using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Bottles.Exploding;
using Bottles.Zipping;
using FubuCore;
using FubuMVC.Core;

namespace Fubu
{
    public class ValidateTargetPathStep : ITemplateStep
    {
        // Error code for file exists http://msdn.microsoft.com/en-us/library/aa232676(v=vs.60).aspx
        public const int DIRECTORY_ALREADY_EXISTS = 58;
        private readonly IFileSystem _fileSystem;

        public ValidateTargetPathStep(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public void Execute(TemplatePlanContext context)
        {
            if(_fileSystem.DirectoryExists(context.TargetPath))
            {
                throw new FubuException(DIRECTORY_ALREADY_EXISTS, "Directory: {0} already exists", context.TargetPath);
            }
        }
    }

    public class UnzipTemplateStep : ITemplateStep
    {
        public static readonly string TemplateZip = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "defaultTemplate.zip");
        private readonly IZipFileService _zipFileService;

        public UnzipTemplateStep(IZipFileService zipFileService)
        {
            _zipFileService = zipFileService;
        }

        public void Execute(TemplatePlanContext context)
        {
            var zipPath = context.Input.ZipFlag;
            var templateZip = string.IsNullOrEmpty(zipPath)
                                ? TemplateZip
                                : Path.Combine(Environment.CurrentDirectory, zipPath);

            _zipFileService.ExtractTo(templateZip, context.TargetPath, ExplodeOptions.PreserveDestination);
        }
    }

    public class CloneGitRepositoryTemplateStep : ITemplateStep
    {
        private readonly IProcessFactory _processFactory;

        public CloneGitRepositoryTemplateStep(IProcessFactory processFactory)
        {
            _processFactory = processFactory;
        }

        public void Execute(TemplatePlanContext context)
        {
            var gitProcess = _processFactory
                .Create(p =>
                            {
                                p.UseShellExecute = false;
                                p.FileName = "git";
                                p.Arguments = "clone {0} {1}".ToFormat(context.Input.GitFlag, context.TargetPath);
                            });

            gitProcess.Start();
            gitProcess.WaitForExit();
            if (gitProcess.ExitCode != 0)
            {
                throw new FubuException(gitProcess.ExitCode, "Command finished with a non-zero exit code");
            }
        }
    }

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
                _fileSystem.MoveFile(file, newFileName);
            }
        }
    }
}