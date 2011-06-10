using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FubuCore.CommandLine;
using Ionic.Zip;

namespace Fubu
{
    [CommandDescription("Creates a new FubuMVC project", Name = "new")]
    public class NewCommand : FubuCommand<NewCommandInput>
    {
        private Dictionary<string, string> _templateKeywords;
        private string _newProjectPath;
        private const string DELIMITER = "@@";
        private static string TEMPLATEZIP = Path.Combine(Assembly.GetExecutingAssembly().Location, "fubuTemplate.zip");

        public override bool Execute(NewCommandInput input)
        {
            _templateKeywords = new Dictionary<string, string>
                                {
                                    { "PROJECTNAME", input.ProjectName },
                                    { "SHORTNAME", input.ProjectName.Split('.').Last() },
                                    { "PROJECTGUID", Guid.NewGuid().ToString() }
                                };
            _newProjectPath = Path.Combine(Environment.CurrentDirectory, input.ProjectName);

            var templateZip = input.ZipFlag ?? TEMPLATEZIP;
            using (var zip = new ZipFile(templateZip))
            {
               zip.ExtractAll(_newProjectPath); 
            }

            var templateDirectory = new DirectoryInfo(input.ProjectName);
            ParseDirectory(templateDirectory);

            return true;
        }

        public void ParseDirectory(DirectoryInfo directory)
        {
            var newDirectoryName = ReplaceKeywords(directory.FullName);
            if (directory.FullName != newDirectoryName)
            {
                directory.MoveTo(newDirectoryName);
                directory.Refresh();
            }

            var files = directory.EnumerateFileSystemInfos();
            files.OfType<FileInfo>().Each(ParseFile);
            files.OfType<DirectoryInfo>().Each(ParseDirectory);
        }

        public void ParseFile(FileInfo file)
        {
            // Ignore files larger than 1MB
            if (file.Length > (1024 * 1024))
            {
                return;
            }


            string fileContent;
            using (var reader = new StreamReader(file.Open(FileMode.Open, FileAccess.Read)))
            {
                fileContent = reader.ReadToEnd();
            }

            var replacedFileContent = ReplaceKeywords(fileContent);
            if (fileContent == replacedFileContent)
            {
                return;
            }

            using (var writer = new StreamWriter(file.Open(FileMode.Truncate, FileAccess.Write)))
            {
                writer.Write(replacedFileContent);
            }

            var newFileName = ReplaceKeywords(file.FullName);
            if (file.FullName != newFileName)
            {
                file.MoveTo(newFileName);
            }
        }

        public string ReplaceKeywords(string input)
        {
            return _templateKeywords.Aggregate(input, (memo, keyword) => memo.Replace(DELIMITER + keyword.Key + DELIMITER, keyword.Value));
        }
    }
}