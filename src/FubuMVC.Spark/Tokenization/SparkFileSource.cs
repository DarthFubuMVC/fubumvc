using System.Collections.Generic;
using System.Linq;
using Bottles;
using FubuCore;
using FubuMVC.Spark.Tokenization.Model;
using FubuMVC.Spark.Tokenization.Scanning;

namespace FubuMVC.Spark.Tokenization
{
    public interface ISparkFileSource
    {
        IEnumerable<SparkFile> GetFiles();
    }

    public class SparkFileSource : ISparkFileSource
    {
        private readonly IFileScanner _fileScanner;
        private readonly IEnumerable<IPackageInfo> _packages;

        public SparkFileSource(IFileScanner fileScanner, IEnumerable<IPackageInfo> packages)
        {
            _fileScanner = fileScanner;
            _packages = packages;
        }

        public IEnumerable<SparkFile> GetFiles()
        {
            var files = new List<SparkFile>();

            var request = buildRequest(files);
            _fileScanner.Scan(request);

            return files;
        }

        private ScanRequest buildRequest(ICollection<SparkFile> files)
        {
            var request = new ScanRequest();
            request.AddFileFilter("*.spark");

            var roots = rootSources().ToList();
            roots.Select(x => x.Path).Each(request.AddRoot);

            request.AddHandler(fileFound =>
            {
                var origin = roots.First(x => x.Path == fileFound.Root).Origin;
                var sparkFile = new SparkFile(fileFound.Path, fileFound.Root, origin);
                
                files.Add(sparkFile);
            });

            return request;
        }

        private IEnumerable<RootSource> rootSources()
        {
            var roots = new List<RootSource>();
            foreach (var package in _packages)
            {
                var pck = package;
                package.ForFolder(BottleFiles.WebContentFolder, file =>
                {
                    var root = new RootSource
                    {
                        Origin = pck.Name, 
                        Path = file
                    };

                    roots.Add(root);
                });
            }

            roots.Add(new RootSource { Origin = "Host", Path = "~/".ToPhysicalPath() });
            return roots;
        }

        #region Nested Type : RootSource
        
        public class RootSource
        {
            public string Path { get; set; }
            public string Origin { get; set; }
        }
        
        #endregion
    }
}