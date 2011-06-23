using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using FubuCore;
using FubuCore.CommandLine;
using FubuMVC.Core;
using Ionic.Zip;

namespace Fubu
{
    [CommandDescription("Creates a new FubuMVC solution", Name = "new")]
    public class NewCommand : FubuCommand<NewCommandInput>
    {
        private Dictionary<string, string> _templateKeywords;
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
        }

        public IFileSystem FileSystem { get; set; }

        public override bool Execute(NewCommandInput input)
        {
            _templateKeywords = new Dictionary<string, string>
                                {
                                    { "FUBUPROJECTNAME", input.ProjectName },
                                    { "FUBUPROJECTSHORTNAME", input.ProjectName.Split('.').Last() },
                                    { "FACEFACE-FACE-FACE-FACE-FACEFACEFACE", Guid.NewGuid().ToString().ToUpper() }
                                };
            var projectPath = Path.Combine(Environment.CurrentDirectory, input.ProjectName);
            if (string.IsNullOrEmpty(input.GitFlag))
            {
                Unzip(input.ZipFlag, projectPath);
            }
            else
            {
                var proc = new Process();
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.FileName = "git";
                proc.StartInfo.Arguments = string.Format("clone {0} {1}", input.GitFlag, projectPath);

                proc.Start();
                proc.WaitForExit();
                if (proc.ExitCode != 0)
                {
                    throw new FubuException(proc.ExitCode, "Command finished with a non-zero exit code");
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

            using (var zip = ZipFile.Read(templateZip))
            {
                Console.WriteLine("Unzipping: {0}", Path.GetFileName(templateZip));
                zip.ZipError += (sender, error) => Console.WriteLine("Error Unzipping {0} - {1}", error.FileName, error.Exception);
                zip.ExtractAll(destination);
            }
        }

        public void ParseDirectory(string directory)
        {
            var newDirectoryName = ReplaceKeywords(directory);
            if (directory != newDirectoryName)
            {
                Console.WriteLine("{0} -> {1}",directory, Path.GetFileName(newDirectoryName));
                FileSystem.MoveDirectory(directory, newDirectoryName);
                directory = newDirectoryName;
            }

            FileSystem.FindFiles(directory, TopLevelFileSet).Each(ParseFile);
            FileSystem.ChildDirectoriesFor(directory).Each(ParseDirectory);
        }

        public void ParseFile(string file)
        {
            string fileContent = string.Empty;

            FileSystem.ReadTextFile(file, c =>
            {
                fileContent = c;
            });

            var replacedFileContent = ReplaceKeywords(fileContent);
            if (fileContent != replacedFileContent)
            {
                FileSystem.WriteStringToFile(file, replacedFileContent);
            }

            var newFileName = ReplaceKeywords(file);
            if (file != newFileName)
            {
                Console.WriteLine("{0} -> {1}", file, Path.GetFileName(newFileName));
                FileSystem.MoveFile(file, newFileName);
            }
        }

        public string ReplaceKeywords(string input)
        {
            return _templateKeywords.Aggregate(input, (memo, keyword) => memo.Replace(keyword.Key, keyword.Value));
        }
    }
}