using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace FubuCore
{
    public interface IFileSystem
    {
        bool FileExists(params string[] path);
        void PersistToFile(object target, params string[] filename);
        T LoadFromFile<T>(params string[] filename) where T : new();
        void OpenInNotepad(params string[] parts);
        void CreateDirectory(string directory);
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


        public bool FileExists(params string[] path)
        {
            return File.Exists(Path.Combine(path));
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
            var filename = Path.Combine(paths);
            Console.WriteLine("Saving to " + filename);
            var serializer = new XmlSerializer(target.GetType());

            using (var stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                serializer.Serialize(stream, target);
            }
        }

        public T LoadFromFile<T>(params string[] paths) where T : new()
        {
            var filename = Path.Combine(paths);

            if (!FileExists(filename)) return new T();

            var serializer = new XmlSerializer(typeof(T));

            using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                return (T)serializer.Deserialize(stream);
            }
        }

        public void OpenInNotepad(params string[] parts)
        {
            string filename = Path.Combine(parts);
            Process.Start("notepad", filename);
        }

        //public void ClearFolder(string directory)
        //{
        //    DeleteFolder(directory);
        //    CreateDirectory(directory);
        //}

        public void DeleteFile(params string[] path)
        {
            var filename = Path.Combine(path);
            if (!File.Exists(filename)) return;

            File.Delete(filename);
        }
    }
}