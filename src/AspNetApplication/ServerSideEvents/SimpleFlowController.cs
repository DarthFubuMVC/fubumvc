using System.Net;
using FubuMVC.Core.Runtime;

namespace AspNetApplication.ServerSideEvents
{
    public class SimpleInput
    {
        public string Contents { get; set;}
    }

    public class SimpleFlowController
    {
        private readonly IOutputWriter _writer;

        public SimpleFlowController(IOutputWriter writer)
        {
            _writer = writer;
        }

        public void Write(SimpleInput input)
        {
            _writer.ContentType(MimeType.EventStream);

            _writer.Write("data: Hello!\n\n");
            _writer.Write("data: Again\n\n");
            _writer.Write("data: Beatles\n\n");
            _writer.Write("data: Rolling Stones\n\n");
            _writer.Write("data: Eagles\n\n");
        }
    }
}