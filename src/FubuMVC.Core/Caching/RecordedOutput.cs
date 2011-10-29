using System;
using System.Collections.Generic;
using System.IO;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Headers;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Caching
{
    public class RecordedOutput : IOutputState, IRecordedOutput
    {
        private readonly IList<IRecordedHttpOutput> _outputs = new List<IRecordedHttpOutput>();

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
            output = new WriteText(renderedOutput);
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

        public void Replay(IHttpWriter writer)
        {
            _outputs.Each(x => x.Replay(writer));
        }

        public void AddOutput(IRecordedHttpOutput output)
        {
            _outputs.Add(output);
        }
    }
}