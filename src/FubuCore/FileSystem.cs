using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using System.Linq;

namespace FubuCore
{
    public interface IFileSystem
    {
        bool FileExists(string filename);
        void PersistToFile(object target, string filename);
        T LoadFromFile<T>(string filename) where T : new();
        void LaunchEditor(string filename);
        void CreateDirectory(string directory);
        void DeleteFile(string filename);
        IEnumerable<string> ChildDirectoriesFor(string directory);
        IEnumerable<string> FindFiles(string directory, FileSet searchSpecification);
        void WriteStringToFile(string filename, string text);
        string ReadStringFromFile(string filename);
        void DeleteDirectory(string directory);
    }

    public static class FileSystemExtensions
    {
        public static void LaunchEditor(this IFileSystem fileSystem, params string[] pathParts)
        {
            fileSystem.LaunchEditor(FileSystem.Combine(pathParts));
        }
        public static bool FileExists(this IFileSystem fileSystem, params string[] pathParts)
        {
            return fileSystem.FileExists(FileSystem.Combine(pathParts));
        }
        public static T LoadFromFile<T>(this IFileSystem fileSystem, params string[] pathParts) where T : new()
        {
            return fileSystem.LoadFromFile<T>(FileSystem.Combine(pathParts));
        }
        public static IEnumerable<string> ChildDirectoriesFor(this IFileSystem fileSystem, params string[] pathParts)
        {
            return fileSystem.ChildDirectoriesFor(FileSystem.Combine(pathParts));
        }
        public static IEnumerable<string> FileNamesFor(this IFileSystem fileSystem, FileSet set, params string[] pathParts)
        {
            return fileSystem.FindFiles(FileSystem.Combine(pathParts), set);
        }

        public static string ReadStringFromFile(this IFileSystem fileSystem, params string[] pathParts)
        {
            return fileSystem.ReadStringFromFile(FileSystem.Combine(pathParts));
        }

        public static void PersistToFile(this IFileSystem fileSystem, object target, params string[] pathParts)
        {
            fileSystem.PersistToFile(target, FileSystem.Combine(pathParts));
        }
        public static void DeleteDirectory(this IFileSystem fileSystem, params string[] pathParts)
        {
            fileSystem.DeleteDirectory(FileSystem.Combine(pathParts));
        }
    }

    public class FileSystem : IFileSystem
    {
        public void CreateDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                return;
            }

            Directory.CreateDirectory(path);
        }

        public static string Combine(params string[] paths)
        {
            return paths.Aggregate(Path.Combine);
        }

        public bool FileExists(string filename)
        {
            return File.Exists(filename);
        }

        public void WriteStringToFile(string filename, string text)
        {
            File.WriteAllText(filename, text);
        }

        public string ReadStringFromFile(string filename)
        {
            return File.ReadAllText(filename);
        }

        public void DeleteDirectory(string directory)
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }
        }

        public void PersistToFile(object target, string filename)
        {
            Debug.WriteLine("Saving to " + filename);
            var serializer = new XmlSerializer(target.GetType());

            using (var stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                serializer.Serialize(stream, target);
            }
        }

        public T LoadFromFile<T>(string filename) where T : new()
        {
            if (!FileExists(filename)) return new T();

            var serializer = new XmlSerializer(typeof(T));

            using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    return (T)serializer.Deserialize(stream);
                }
                catch (Exception e)
                {
                    var message = "Unable to deserialize the contents of file {0} into an instance of type {1}"
                        .ToFormat(filename, typeof (T).FullName);
                    throw new ApplicationException(message, e);
                }
            }
        }

        public void LaunchEditor(string filename)
        {
            Process.Start("notepad", filename);
        }

        public void DeleteFile(string filename)
        {
            if (!File.Exists(filename)) return;

            File.Delete(filename);
        }

        public IEnumerable<string> ChildDirectoriesFor(string directory)
        {
            if (Directory.Exists(directory))
            {
                return Directory.GetDirectories(directory);
            }

            return new string[0];
        }

        // Only here for mocking/stubbing file system junk
        public IEnumerable<string> FindFiles(string directory, FileSet searchSpecification)
        {
            return searchSpecification.IncludedFilesFor(directory);
        }
    }
}