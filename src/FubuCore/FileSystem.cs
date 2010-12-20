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
        bool FileExists(params string[] path);
        void PersistToFile(object target, params string[] filename);
        T LoadFromFile<T>(params string[] filename) where T : new();
        void OpenInNotepad(params string[] parts);
        void CreateDirectory(string directory);
        string Combine(params string[] paths);
        void DeleteFile(params string[] path);

        IEnumerable<string> FileNamesFor(FileSet set, string path);
        void WriteStringToFile(string text, string filename);
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


        //public void DeleteFolder(string folder)
        //{
        //    if (Directory.Exists(folder))
        //    {
        //        Directory.Delete(folder, true);
        //    }
        //}


        //public string[] GetSubFolders(string folderPath)
        //{
        //    if (!Directory.Exists(folderPath))
        //    {
        //        return new string[0];
        //    }


        //    string[] strings = Directory.GetDirectories(folderPath);
        //    var suiteFolders = new List<string>();
        //    foreach (string folder in strings)
        //    {
        //        var dir = new DirectoryInfo(folder);
        //        //Ignore directories that are hidden that have other FileAttributes
        //        if (
        //            (dir.Attributes & (FileAttributes.Hidden | FileAttributes.Directory)) ==
        //            (FileAttributes.Hidden | FileAttributes.Directory))
        //        {
        //            continue;
        //        }

        //        suiteFolders.Add(folder);
        //    }


        //    return suiteFolders.ToArray();
        //}

        //public string[] GetFiles(string folderPath, string extensionWithoutPeriod)
        //{
        //    if (!Directory.Exists(folderPath))
        //    {
        //        return new string[0];
        //    }

        //    return Directory.GetFiles(folderPath, "*." + extensionWithoutPeriod);
        //}

        string IFileSystem.Combine(params string[] paths)
        {
            return Combine(paths);
        }

        public static string Combine(params string[] paths)
        {
            if (paths.Length == 0) return string.Empty;
            if (paths.Length == 1) return paths[0];

            var queue = new Queue<string>(paths);
            var result = queue.Dequeue();

            while (queue.Any())
            {
                result = Path.Combine(result, queue.Dequeue());
            }

            return result;
        }

        public bool FileExists(params string[] path)
        {
            return File.Exists(Combine(path));
        }

        public void WriteStringToFile(string text, string filename)
        {
            using (var writer = new StreamWriter(filename))
            {
                writer.Write(text);
                writer.Close();
            }
        }

        public string ReadStringFromFile(string filename)
        {
            using (var reader = new StreamReader(filename))
            {
                return reader.ReadToEnd();
            }
        }

        //public void SaveAndOpenHtml(string html)
        //{
        //    string fileName = Path.GetTempPath() + ".htm";
        //    WriteStringToFile(html, fileName);
        //    Process.Start(fileName);
        //}

        public void PersistToFile(object target, params string[] paths)
        {
            var filename = Combine(paths);
            Console.WriteLine("Saving to " + filename);
            var serializer = new XmlSerializer(target.GetType());

            using (var stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                serializer.Serialize(stream, target);
            }
        }

        public T LoadFromFile<T>(params string[] paths) where T : new()
        {
            var filename = Combine(paths);

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

        public void OpenInNotepad(params string[] parts)
        {
            string filename = Combine(parts);
            Process.Start("notepad", filename);
        }

        //public void ClearFolder(string directory)
        //{
        //    DeleteFolder(directory);
        //    CreateDirectory(directory);
        //}

        public void DeleteFile(params string[] path)
        {
            var filename = Combine(path);
            if (!File.Exists(filename)) return;

            File.Delete(filename);
        }

        // Only here for mocking/stubbing file system junk
        public IEnumerable<string> FileNamesFor(FileSet set, string path)
        {
            return set.IncludedFilesFor(path);
        }
    }
}