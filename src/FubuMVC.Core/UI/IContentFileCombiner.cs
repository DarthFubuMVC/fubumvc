using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using FubuCore;
using FubuMVC.Core.Packaging;

namespace FubuMVC.Core.UI
{
    public interface IContentFileCombiner
    {
        string GenerateCombinedFile(IEnumerable<string> filenames, string separatorFormat = null);
    }

    public class ContentFileCombiner : IContentFileCombiner
    {
        private const string CombinedFileFolder = "_combined";
        private readonly IFileSystem _fileSystem;

        public ContentFileCombiner(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public string GenerateCombinedFile(IEnumerable<string> filenames, string separatorFormat = null)
        {
            if (!filenames.Any()) return null;
            var extension = Path.GetExtension(filenames.First());
            var combinedName = getCombinedName(filenames, extension);
            var outputFile = FileSystem.Combine(FubuMvcPackageFacility.GetApplicationPath(), "content", CombinedFileFolder, combinedName);
            if (!_fileSystem.FileExists(outputFile))
            {
                writeFile(outputFile, filenames, separatorFormat);
            }
            return CombinedFileFolder + "/" + combinedName;
        }

        private static string getCombinedName(IEnumerable<string> rawFiles, string extension)
        {
            var name = rawFiles.Select(x => x.ToLowerInvariant()).OrderBy(x => x).Join("*");
            return MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(name)).Select(b => b.ToString("x2")).Join("") + extension;
        }

        private void writeFile(string outputFile, IEnumerable<string> sourceFiles, string separatorFormat)
        {
            Debug.WriteLine("generating combined file: " + outputFile);
            _fileSystem.CreateDirectory(Path.GetDirectoryName(outputFile));
            using (var output = File.CreateText(outputFile))
            {
                foreach (var sourceFile in sourceFiles)
                {
                    var readAllText = File.ReadAllText(sourceFile);
                    if (separatorFormat != null)
                    {
                        var separator = separatorFormat.ToFormat(Path.GetFileName(sourceFile));
                        output.WriteLine(separator);
                    }
                    output.WriteLine(readAllText);
                }
            }
        }
    }

}