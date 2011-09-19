using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using FubuMVC.Core.Rest.Projections;

namespace FubuMVC.Tests.Rest.Projections
{
    public class InMemoryMediaNode : IMediaNode
    {
        private readonly IDictionary<string, object> _properties 
            = new Dictionary<string, object>();

        public IMediaNode AddChild(string name)
        {
            throw new NotImplementedException();
        }

        public void SetAttribute(string name, object value)
        {
            _properties.Add(name, value);
        }

        public void WriteLinks(IEnumerable<SyndicationLink> links)
        {
            throw new NotImplementedException();
        }

        public object PropFor(string name)
        {
            return _properties[name];
        }
    }
}