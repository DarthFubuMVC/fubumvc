using System.IO;
using System.Threading;
using FubuCore;

namespace Fubu.Generation
{
    // TODO -- eliminate this and use within FubuCore
    public static class FileSystemExtensions
    {
        // Basic integration coverage on this but having to rely mostly on manual testing here
        public static void ForceClean(this IFileSystem system, string path)
        {
            if (path.IsEmpty()) return;
            if (!Directory.Exists(path)) return;

            try
            {
                cleanDirectory(path, false);
            }
            catch
            {
                // just retry it
                cleanDirectory(path, false);
            }
        }

        private static void cleanDirectory(string directory, bool remove = true)
        {
            string[] files = Directory.GetFiles(directory);
            string[] children = Directory.GetDirectories(directory);

            foreach (var file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            Thread.Sleep(10);

            foreach (var child in children)
            {
                cleanDirectory(child);
            }

            if (remove)
            {
                Thread.Sleep(10);
                Directory.Delete(directory, false);
            }
        }
    }
}