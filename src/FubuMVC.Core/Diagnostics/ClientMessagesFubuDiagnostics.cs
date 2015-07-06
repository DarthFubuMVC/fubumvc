using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Runtime.Aggregation;

namespace FubuMVC.Core.Diagnostics
{
    public class ClientMessagesFubuDiagnostics
    {
        private readonly IClientMessageCache _messages;

        public ClientMessagesFubuDiagnostics(IClientMessageCache messages)
        {
            _messages = messages;
        }

        public ClientMessageReport get_clientmessages()
        {
            return new ClientMessageReport
            {
                messages = _messages.AllClientMessages().OrderBy(x => x.Message).Select(x =>
                {
                    var description1 = new ClientMessageDescription(x);


                    return description1;
                }).ToArray()
            };
        }
    }

    public class ClientMessageReport
    {
        public ClientMessageDescription[] messages;
    }

    public class ClientMessageDescription
    {
        public ClientMessageDescription()
        {
        }

        public ClientMessageDescription(ClientMessagePath path)
        {
            type = path.Message;
            query = path.InputType == null ? "N/A" : path.InputType.FullName;
            resource = path.ResourceType.FullName;
            chain = path.Chain.GetHashCode();
        }

        public int chain { get; set; }

        public string resource { get; set; }

        public string query { get; set; }

        public string type { get; set; }
    }
}