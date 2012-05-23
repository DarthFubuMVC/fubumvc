using FubuMVC.Core.Runtime;

namespace AspNetApplication.ServerSideEvents
{
    public interface IServerEventWriter
    {
        void WriteData(string data, string id = null, string @event = null, int? retry = null);
        void Write(ServerEvent @event);
    }

    public class ServerEventWriter : IServerEventWriter
    {
        public readonly string Data = "data: ";
        public readonly string Event = "event: ";
        public readonly string Id = "id: ";
        public readonly string Retry = "retry: ";

        private readonly IOutputWriter _writer;
        private bool _first = true;


        public ServerEventWriter(IOutputWriter writer)
        {
            _writer = writer;
        }

        public void WriteData(string data, string id = null, string @event = null, int? retry = null)
        {
            if (_first)
            {
                _writer.ContentType(MimeType.EventStream);
                _first = false;
            }
            
            writeProp(Id, id);
            writeProp(Event, @event);
            writeProp(Retry, retry);
            writeProp(Data, data);
            _writer.Write("\n");
            _writer.Flush();
        }

        public void Write(ServerEvent @event)
        {
            WriteData(@event.Data, @event.Id, @event.Event, @event.Retry);
        }

        private void writeProp(string flag, object text)
        {
            if (text == null) return;

            _writer.Write(flag);
            _writer.Write(text.ToString());
            _writer.Write("\n");
        }
    }
}