using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using System.Linq;

namespace FubuCore
{
    public interface IFileSystem
    {
        bool FileExists(string filename);
        void DeleteFile(string filename);
        void MoveFile(string from, string to);
        void MoveDirectory(string from, string to);
        bool IsFile(string path);

        string GetFullPath(string path);


        void Copy(string source, string destination);

        void WriteStreamToFile(string filename, Stream stream);
        void WriteStringToFile(string filename, string text);
        void AppendStringToFile(string filename, string text);

        string ReadStringFromFile(string filename);
        void WriteObjectToFile(string filename, object target);
        T LoadFromFile<T>(string filename) where T : new();

        void CreateDirectory(string directory);

        /// <summary>
        /// Deletes the directory
        /// </summary>
        void DeleteDirectory(string directory);

        /// <summary>
        /// Deletes the directory to clear the content
        /// Then recreates it. An empty clean, happy, directory.
        /// </summary>
        /// <param name="directory"></param>
        void CleanDirectory(string directory);

        bool DirectoryExists(string directory);

        void LaunchEditor(string filename);
        IEnumerable<string> ChildDirectoriesFor(string directory);
        IEnumerable<string> FindFiles(string directory, FileSet searchSpecification);

        void ReadTextFile(string path, Action<string> reader);
        void MoveFiles(string from, string to);

        string GetDirectory(string path);
        string GetFileName(string path);

        void AlterFlatFile(string path, Action<List<string>> alteration);
    }

    public static class FileSystemExtensions
    {
        public static bool DirectoryExists(this IFileSystem fileSystem, params string[] pathParts)
        {
            return fileSystem.DirectoryExists(FileSystem.Combine(pathParts));
        }

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

        public static void CreateDirectory(this IFileSystem fileSystem, params string[] pathParts)
        {
            fileSystem.CreateDirectory(FileSystem.Combine(pathParts));
        }
    }

    public class FileSystem : IFileSystem
    {
        public void CreateDirectory(string path)
        {
            if (path.IsEmpty()) return;

            if (Directory.Exists(path))
            {
                return;
            }

            Directory.CreateDirectory(path);
        }

        public void Copy(string source, string destination)
        {
            if(IsFile(source))
                internalFileCopy(source, destination);
            else
                internalDirectoryCopy(source, destination);
        }

        void internalFileCopy(string source, string destination)
        {
            var fileName = Path.GetFileName(source);

            var fullSourcePath = Path.GetFullPath(source);
            var fullDestPath = Path.GetFullPath(destination);


            var isFile = destinationIsFile(source, destination);

            string destinationDir = fullDestPath;
            if(isFile)
                destinationDir = Path.GetDirectoryName(fullDestPath);

            CreateDirectory(destinationDir);

            if(!isFile) //aka its a directory
                fullDestPath = Combine(fullDestPath, fileName);

            try
            {
                File.Copy(fullSourcePath, fullDestPath, true);
            }
            catch (Exception ex)
            {
                var msg = "Was trying to copy '{0}' to '{1}' and encountered an error. :(".ToFormat(fullSourcePath, fullDestPath);
                throw new Exception(msg, ex);
            }
        }

        void internalDirectoryCopy(string source, string destination)
        {
            var files = Directory.GetFiles(source, "*.*", SearchOption.AllDirectories);
            files.Each(f =>
                {
                    //need to test this for name correctness
                    var destName = Combine(destination, Path.GetFileName(f));
                    internalFileCopy(f, destName);
                });
        }

        bool destinationIsFile(string source, string destination)
        {
            if(FileExists(destination) || DirectoryExists(destination))
            {
                //it exists 
                return IsFile(destination);
            }

            if(destination.Last() == Path.DirectorySeparatorChar)
            {
                //last char is a '/' so its a directory
                return false;
            }

            //last char is not a '/' so its a file
            return true;
        }

        public bool IsFile(string path)
        {
            //resolve the path
            path = Path.GetFullPath(path);

            if (!File.Exists(path) && !Directory.Exists(path))
                throw new IOException("This path '{0}' doesn't exist!".ToFormat(path));

            var attr = File.GetAttributes(path);
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                return false;
            }
            return true;
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

        public void AppendStringToFile(string filename, string text)
        {
            File.AppendAllText(filename, text);
        }


        public string ReadStringFromFile(string filename)
        {
            return File.ReadAllText(filename);
        }

        public string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        public void AlterFlatFile(string path, Action<List<string>> alteration)
        {
            var list = new List<string>();

            if (FileExists(path))
            {
                ReadTextFile(path, list.Add);
            }

            list.RemoveAll(x => x.Trim() == string.Empty);

            alteration(list);

            using (var writer = new StreamWriter(path))
            {
                list.Each(x => writer.WriteLine(x));
            }
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
            if (directory.IsEmpty()) return;


            DeleteDirectory(directory);
            Thread.Sleep(10);

            CreateDirectory(directory);
        }

        public bool DirectoryExists(string directory)
        {
            return Directory.Exists(directory);
        }

        public void WriteObjectToFile(string filename, object target)
        {
            Debug.WriteLine("Saving to " + filename);
            var serializer = new XmlSerializer(target.GetType());

            CreateDirectory(GetDirectory(filename));

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

        public void LaunchBrowser(string filename)
        {
            Process.Start("explorer", filename);
        }

        public void DeleteFile(string filename)
        {
            if (!File.Exists(filename)) return;

            File.Delete(filename);
        }

        public void MoveFile(string from, string to)
        {
            CreateDirectory(Path.GetDirectoryName(to));

            try
            {
                File.Move(from, to);
            }
            catch (IOException ex)
            {
                var msg = "Trying to move '{0}' to '{1}'".ToFormat(from, to);
                throw new Exception(msg, ex);
            }
        }

        public void MoveFiles(string from, string to)
        {
            var files = Directory.GetFiles(from, "*.*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var partialPath = file.Replace(from, "");
                if (partialPath.StartsWith(@"\")) partialPath = partialPath.Substring(1);
                var newPath = FileSystem.Combine(to, partialPath);
                MoveFile(file, newPath);
            }
        }

        public void MoveDirectory(string from, string to)
        {
            Directory.Move(from, to);
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
            if (!FileExists(path)) return;

            using (var reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    callback(line);
                }
            }
        }
        
        public string GetFullPath(string path)
        {
            return Path.GetFullPath(path);
        }

        public static IEnumerable<string> GetChildDirectories(string directory)
        {
            if (!Directory.Exists(directory))
                return new string[0];


            return Directory.GetDirectories(directory);
        }

        public string GetDirectory(string path)
        {
            return Path.GetDirectoryName(path);
        }

    }
}