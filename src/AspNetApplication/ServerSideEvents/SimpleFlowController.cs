using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FubuCore.Binding;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;
using FubuCore;

namespace AspNetApplication.ServerSideEvents
{
    public class SimpleInput : ServerSideInput
    {
        public string Contents { get; set; }
    }

    public class ServerSideInput
    {
        [HeaderValue("Last-Event-ID")]
        public string LastEventId { get; set; }
    }

    public class HeaderValueAttribute : BindingAttribute
    {
        private readonly string _headerName;

        public HeaderValueAttribute(string headerName)
        {
            _headerName = headerName;
        }

        public HeaderValueAttribute(HttpRequestHeader header)
        {
            _headerName = HttpRequestHeaders.HeaderNameFor(header);
        }

        public override void Bind(PropertyInfo property, IBindingContext context)
        {
            context.Service<IRequestHeaders>().Value<string>(_headerName, val =>
            {
                property.SetValue(context.Object, val, null);
            });
        }
    }

    public class ServerEvent
    {
        public string Data { get; set; }
        public string Id { get; set; }
        public string Event { get; set; }
        public int? Retry { get; set; }
    }


    public class ServerEventWriter
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

    public class SimpleFlowController
    {
        private readonly ServerEventWriter _writer;

        private static readonly IList<string> _bands = new List<string>{
            "Eagles", "Steve Miller Band", "ZZ Top", "Credence Clearwater Revival", "Lynard Skynard", "Bad Company", "Black Keys", "Neil Young"
            , "Tom Jones", "John Cougar Mellencamp", "Beatles", "The Head and the Heart"
    };

        public SimpleFlowController(ServerEventWriter writer)
        {
            _writer = writer;
        }

        public Task get_events_simple(SimpleInput input)
        {
            return Task.Factory.StartNew(() =>
            {
                var index = input.LastEventId.IsEmpty() ? 0 : int.Parse(input.LastEventId);
                var next = index + 4;
                if (next > _bands.Count)
                {
                    next = _bands.Count;
                }


                for (int i = index + 1; i < next; i++)
                {
                    _writer.WriteData(_bands[i], id: i.ToString());
                    Thread.Sleep(2000);
                }
            });


        }
    }


}