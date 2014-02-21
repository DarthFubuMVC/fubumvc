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
    public class RecordedOutput : IOutputState, IRecordedOutput, IHaveContentType
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

        public void Write(string renderedOutput)
        {
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

        public void Flush()
        {
            // no-op;
        }



        public void Replay(IHttpResponse response)
        {
            _outputs.Each(x => x.Replay(response));
        }



        public string GetText()
        {
            var writer = new StringWriter();
            _outputs.OfType<IRecordedTextOutput>().Each(o => o.WriteText(writer));
            return writer.ToString();
        }

        public IEnumerable<Header> Headers()
        {
            return _outputs.OfType<Header>();
        }

        public bool IsEmpty()
        {
            return !_outputs.Any();
        }

        public void AddOutput(IRecordedHttpOutput output)
        {
            _outputs.Add(output);
        }

        public string ContentType
        {
            get
            {
                var o = _outputs.OfType<IHaveContentType>().Where(x => x.ContentType.IsNotEmpty()).LastOrDefault();
                return o == null ? null : o.ContentType;
            
            }
        }
    }
}