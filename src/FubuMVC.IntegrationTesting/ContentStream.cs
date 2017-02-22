using System.IO;
using FubuCore;

namespace FubuMVC.IntegrationTesting
{
    public class ContentStream
    {
        private readonly string _path;
        private readonly StringWriter _writer = new StringWriter();

        public ContentStream(string folder, string name, string extension)
        {
            _path = Path.Combine(folder, name + extension);
        }

        public ContentStream WriteLine(string format, params object[] parameters)
        {
            _writer.WriteLine(format, parameters);
            return this;
        }

        public void Write(string text)
        {
            _writer.WriteLine(text.Replace("'", "\"").TrimStart());
        }

        public void DumpContents()
        {
            new FileSystem().WriteStringToFile(_path, _writer.ToString());
        }
    }
}