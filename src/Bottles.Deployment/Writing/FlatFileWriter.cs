using System;
using System.Collections.Generic;
using System.IO;
using FubuCore;

namespace Bottles.Deployment.Writing
{
    public interface IFlatFileWriter
    {
        void WriteProperty(string name, string value);
        void WriteLine(string line);
        void Sort();
    }

    public class FlatFileWriter : IFlatFileWriter
    {
        private readonly List<string> _list;

        public FlatFileWriter(List<string> list)
        {
            _list = list;
        }

        public void WriteProperty(string name, string value)
        {
            _list.RemoveAll(x => x.StartsWith(name + "="));
            _list.Add("{0}={1}".ToFormat(name, value));
        }

        public void WriteLine(string line)
        {
            _list.Fill(line);
        }

        public void Sort()
        {
            _list.Sort();
        }

        public List<string> List
        {
            get { return _list; }
        }

        public override string ToString()
        {
            var writer = new StringWriter();
            _list.Each(x => writer.WriteLine(x));

            return writer.ToString();
        }
    }

    public static class FileSystemExtensions
    {
        public static void WriteToFlatFile(this IFileSystem system, string path, Action<IFlatFileWriter> configuration)
        {
            system.AlterFlatFile(path, list => configuration(new FlatFileWriter(list)));
        }
    }
}