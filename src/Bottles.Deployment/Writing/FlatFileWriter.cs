using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuCore.CommandLine;

namespace Bottles.Deployment.Writing
{
    public interface IFlatFileWriter
    {
        void WriteProperty(string name, string value);
        void WriteLine(string line);
        void Sort();
        void Describe();
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

        public void Describe()
        {
            _list.Each(ConsoleWriter.Write);
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

        public static void WriteProperty(this IFileSystem system, string path, string propertyText)
        {
            ConsoleWriter.Write("Writing {0} to {1}", path, propertyText);
            system.WriteToFlatFile(path, file =>
            {
                var parts = propertyText.Split('=');
                file.WriteProperty(parts.First(), parts.Last());

                Console.WriteLine("Contents of {0}", path);
                file.Sort();

                file.Describe();

                ConsoleWriter.PrintHorizontalLine();
            });
        }
    }
}