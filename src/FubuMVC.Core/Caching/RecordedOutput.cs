using System;
using System.Collections.Generic;
using System.IO;
using FubuCore;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Headers;
using FubuMVC.Core.Runtime;
using System.Linq;

namespace FubuMVC.Core.Caching
{
    public class RecordedOutput : IOutputState, IRecordedOutput
    {
        private readonly IFileSystem _fileSystem;
        private readonly IList<IRecordedHttpOutput> _outputs = new List<IRecordedHttpOutput>();

        public RecordedOutput(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        private IRecordedHttpOutput output
        {
            set { _outputs.Add(value); }
        }

        public IEnumerable<IRecordedHttpOutput> Outputs
        {
            get { return _outputs; }
        }

        public void Write(string contentType, string renderedOutput)
        {
            output = new SetContentType(contentType);
            output = new WriteTextOutput(renderedOutput);
        }

        public void Write(string contentType, Action<Stream> action)
        {
            output = new SetContentType(contentType);

            var stream = new MemoryStream();
            action(stream);

            output = new WriteStream(stream);
        }

        public void AppendHeader(string header, string value)
        {
            output = new Header(header, value);
        }

        public void WriteFile(string contentType, string localFilePath, string displayName)
        {
            output = WriteFileRecord.Create(_fileSystem, localFilePath, contentType, displayName);
        }

        public void Replay(IHttpWriter writer)
        {
            _outputs.Each(x => x.Replay(writer));
        }

        public void ForHeader(string headerName, Action<string> action)
        {
            var header = _outputs.OfType<Header>().Where(x => x.Matches(headerName)).FirstOrDefault();
            if (header != null)
            {
                action(header.Value);
            }
        }

        public string GetHeaderValue(string headerName)
        {
            string returnValue = null;
            ForHeader(headerName, val => returnValue = val);

            return returnValue;
        }

        public string GetText()
        {
            var writer = new StringWriter();
            _outputs.OfType<IRecordedTextOutput>().Each(o => o.WriteText(writer));
            return writer.ToString();
        }

        public void AddOutput(IRecordedHttpOutput output)
        {
            _outputs.Add(output);
        }
    }
}