using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Aggregation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FubuMVC.Core.Json
{
    public class AggregatedQueryReader : IReader<AggregatedQuery>
    {
        public IEnumerable<string> Mimetypes
        {
            get
            {
                yield return "text/json";
                yield return "application/json";
            }
        }

        public AggregatedQuery Read(string mimeType, IFubuRequestContext context)
        {
            var messageTypes = context.Service<IClientMessageCache>();
            var serializer = context.Service<NewtonSoftJsonSerializer>().InnerSerializer();

            var json = context.Request.Input.ReadAllText();


            return Read(serializer, messageTypes, json);
        }

        public AggregatedQuery Read(JsonSerializer serializer, IClientMessageCache messageTypes, string json)
        {
            var token = JToken.Parse(json);

            var queries = token["queries"] as JArray;

            if (queries == null) throw new ArgumentOutOfRangeException("json","No 'queries' were sent in this request");

            return new AggregatedQuery
            {
                queries = queries.Select(x => readQuery(x, messageTypes, serializer)).ToArray()
            };

        }

        private ClientQuery readQuery(JToken jToken, IClientMessageCache messageTypes, JsonSerializer serializer)
        {
            var messageName = jToken["type"].Value<string>();
            var chain = messageTypes.FindChain(messageName);

            string correlationId = null;
            var correlationIdToken = jToken["correlationId"];
            if (correlationIdToken != null)
            {
                correlationId = correlationIdToken.Value<string>();
            }

            var query = new ClientQuery {type = messageName, correlationId = correlationId};

            if (chain.InputType() != null)
            {
                var reader = new JTokenReader(jToken["query"]);
                query.query = serializer.Deserialize(reader, chain.InputType());
            }

            return query;
        }


    }
}
