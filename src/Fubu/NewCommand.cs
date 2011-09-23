using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Bottles.Exploding;
using Bottles.Zipping;
using FubuCore;
using FubuCore.CommandLine;
using FubuMVC.Core;
using Ionic.Zip;

namespace Fubu
{
    [CommandDescription("Creates a new FubuMVC solution", Name = "new")]
    public class NewCommand : FubuCommand<NewCommandInput>
    {
        private static readonly string TemplateZip = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "fubuTemplate.zip");
        private static readonly FileSet TopLevelFileSet = new FileSet
                                                              {
                                                                  DeepSearch = false,
                                                                  Include = "*.*",
                                                                  Exclude = "*.exe;*.dll"
                                                              };

        public NewCommand()
        {
            FileSystem = new FileSystem();
            ZipService = new ZipFileService(FileSystem);
            KeywordReplacer = new KeywordReplacer();
            ProcessFactory = new ProcessFactory();
        }

        public IFileSystem FileSystem { get; set; }
        public IZipFileService ZipService { get; set; }
        public IKeywordReplacer KeywordReplacer { get; set; }
        public IProcessFactory ProcessFactory { get; set; }

        public override bool Execute(NewCommandInput input)
        {
            var templateKeywords = new Dictionary<string, string>
                                {
                                    { "FUBUPROJECTNAME", input.ProjectName },
                                    { "FUBUPROJECTSHORTNAME", input.ProjectName.Split('.').Last() },
                                    { "FACEFACE-FACE-FACE-FACE-FACEFACEFACE", Guid.NewGuid().ToString().ToUpper() }
                                };
            KeywordReplacer.SetTokens(templateKeywords);

            var projectPath = Path.Combine(Environment.CurrentDirectory, input.ProjectName);
            if (string.IsNullOrEmpty(input.GitFlag))
            {
                Unzip(input.ZipFlag, projectPath);
            }
            else
            {
                var gitProcess = ProcessFactory.Create(p =>
                {
                    p.UseShellExecute = false;
                    p.FileName = "git";
                    p.Arguments = string.Format("clone {0} {1}", input.GitFlag, projectPath);
                });

                gitProcess.Start();
                gitProcess.WaitForExit();
                if (gitProcess.ExitCode != 0)
                {
                    throw new FubuException(gitProcess.ExitCode, "Command finished with a non-zero exit code");
                }
            }

            ParseDirectory(projectPath);

            Console.WriteLine("Solution {0} created", input.ProjectName);
            return true;
        }

        public void Unzip(string zipPath, string destination)
        {
            var templateZip = string.IsNullOrEmpty(zipPath)
                                ? TemplateZip
                                : Path.Combine(Environment.CurrentDirectory, zipPath);

            ZipService.ExtractTo(templateZip, destination, ExplodeOptions.PreserveDestination);
        }

        public void ParseDirectory(string directory)
        {
            var newDirectoryName = KeywordReplacer.Replace(directory);
            if (directory != newDirectoryName)
            {
                Console.WriteLine("{0} -> {1}",directory, FileSystem.GetFileName(newDirectoryName));
                FileSystem.MoveDirectory(directory, newDirectoryName);
                directory = newDirectoryName;
            }

            FileSystem.FindFiles(directory, TopLevelFileSet).Each(ParseFile);
            FileSystem.ChildDirectoriesFor(directory).Each(ParseDirectory);
        }

        public void ParseFile(string file)
        {
            var fileContent = FileSystem.ReadStringFromFile(file);

            var replacedFileContent = KeywordReplacer.Replace(fileContent);
            if (fileContent != replacedFileContent)
            {
                FileSystem.WriteStringToFile(file, replacedFileContent);
            }

            var newFileName = KeywordReplacer.Replace(file);
            if (file != newFileName)
            {
                Console.WriteLine("{0} -> {1}", file, Path.GetFileName(newFileName));
                FileSystem.MoveFile(file, newFileName);
            }
        }
    }
}