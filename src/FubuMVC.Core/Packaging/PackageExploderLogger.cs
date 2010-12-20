using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace FubuMVC.Core.Packaging
{
    public class PackageExploderLogger : IPackageExploderLogger
    {
        private readonly Action<string> _writer;

        public PackageExploderLogger(Action<string> writer)
        {
            _writer = writer;
        }

        private void write(string format, params object[] parameters)
        {
            _writer(format.ToFormat(parameters));
        }

        public void WritePackageDirectoryDeleted(string directoryName)
        {
            write("Deleted exploded package directory {0}", directoryName);
        }

        public void WritePackageZipFileExploded(string zipFile, string directoryName)
        {
            write("Exploded package zip file {0} to {1}", zipFile, directoryName);
        }

        public void WritePackageZipFileWasSameVersionAsExploded(string file)
        {
            write("Current version of package file {0} is already exploded to the application folder", file);
        }

        public void WritePackageZipsFound(string applicationDirectory, IEnumerable<string> packageFileNames)
        {
            if (packageFileNames.Any())
            {
                Console.WriteLine("Found these package zip files:");
                packageFileNames.Each(x => Console.WriteLine("  " + x));
            }
            else
            {
                Console.WriteLine("No package zip files found for the application at {0}", applicationDirectory);
            }
        }


        public void WriteExistingDirectories(string applicationDirectory, IEnumerable<string> existingDirectories)
        {
            if (existingDirectories.Any())
            {
                Console.WriteLine("Found {0} exploded package directories in the application at {0}", applicationDirectory);

                existingDirectories.Each(dir => Console.WriteLine("  " + dir));
            }
            else
            {
                Console.WriteLine("No exploded package directories in the application at {0}", applicationDirectory);
            }
        }
    }
}