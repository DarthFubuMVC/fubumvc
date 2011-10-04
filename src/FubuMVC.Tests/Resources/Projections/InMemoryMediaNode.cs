using System;
using System.Collections.Generic;
using FubuMVC.Core.Resources;
using FubuMVC.Core.Resources.Media;

namespace FubuMVC.Tests.Resources.Projections
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

        public void WriteLinks(IEnumerable<Link> links)
        {
            throw new NotImplementedException();
        }

        public object PropFor(string name)
        {
            return _properties[name];
        }
    }
}