using System.Collections.Generic;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Resources.Conneg.New
{
    public class StringWriter : IMediaWriter<string>
    {
        private readonly IOutputWriter _writer;

        public StringWriter(IOutputWriter writer)
        {
            _writer = writer;
        }

        public void Write(string mimeType, string resource)
        {
            _writer.Write(MimeType.Text, resource);
        }

        public IEnumerable<string> Mimetypes
        {
            get { yield return MimeType.Text.Value; }
        }
    }
}