using System.IO;
using System.Text;

namespace FubuMVC.Core.Runtime
{
    public class InMemoryOutputWriter : IOutputWriter
    {
        private readonly StringBuilder _builder = new StringBuilder();
        private readonly StringWriter _writer;

        public InMemoryOutputWriter()
        {
            _writer = new StringWriter(_builder);
        }

        public void WriteFile(string contentType, string localFilePath, string displayName)
        {
        }

        public void Write(string contentType, string renderedOutput)
        {
            _writer.WriteLine(renderedOutput);
        }

        public void RedirectToUrl(string url)
        {
        }

        public override string ToString()
        {
            return _builder.ToString();
        }
    }
}