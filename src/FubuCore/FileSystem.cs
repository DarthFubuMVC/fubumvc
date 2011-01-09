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
        void DeleteFile(string filename);
        void MoveFile(string from, string to);


        void WriteStreamToFile(string filename, Stream stream);
        void WriteStringToFile(string filename, string text);
        string ReadStringFromFile(string filename);
        void WriteObjectToFile(string filename, object target);
        T LoadFromFile<T>(string filename) where T : new();

        void CreateDirectory(string directory);
        void DeleteDirectory(string directory);
        void CleanDirectory(string directory);
        bool DirectoryExists(string directory);

        void LaunchEditor(string filename);
        IEnumerable<string> ChildDirectoriesFor(string directory);
        IEnumerable<string> FindFiles(string directory, FileSet searchSpecification);

        void ReadTextFile(string path, Action<string> reader);
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
            fileSystem.WriteObjectToFile(FileSystem.Combine(pathParts), target);
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

        public const int BufferSize = 32768;

        public void WriteStreamToFile(string filename, Stream stream)
        {
            CreateDirectory(Path.GetDirectoryName(filename));

            var fileSize = 0;
            using (var fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                int bytesRead;
                var buffer = new byte[BufferSize];
                do
                {
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    fileSize += bytesRead;

                    if (bytesRead > 0)
                    {
                        fileStream.Write(buffer, 0, bytesRead);
                    }
                } while (bytesRead > 0);
                fileStream.Flush();
            }

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

        public void CleanDirectory(string directory)
        {
            Directory.GetDirectories(directory).Each(Directory.Delete);
            Directory.GetFiles(directory).Each(File.Delete);
        }

        public bool DirectoryExists(string directory)
        {
            return Directory.Exists(directory);
        }

        public void WriteObjectToFile(string filename, object target)
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

        public void MoveFile(string from, string to)
        {
            CreateDirectory(Path.GetDirectoryName(to));

            File.Move(from, to);
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

        public void ReadTextFile(string path, Action<string> callback)
        {
            using (var reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    callback(line);
                }
            }
        }
    }
}