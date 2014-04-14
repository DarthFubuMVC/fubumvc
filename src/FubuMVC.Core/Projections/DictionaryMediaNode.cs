using System;
using System.Collections.Generic;
using FubuMVC.Core.Resources.Hypermedia;

namespace FubuMVC.Core.Projections
{
    public class DictionaryMediaNode : IMediaNode
    {
        private readonly IDictionary<string, object> _values;

        public DictionaryMediaNode()
            : this(new Dictionary<string, object>())
        {
        }

        public DictionaryMediaNode(IDictionary<string, object> values)
        {
            _values = values;
        }

        public IDictionary<string, object> Values
        {
            get { return _values; }
        }

        public IMediaNode AddChild(string name)
        {
            var dictionary = new Dictionary<string, object>();
            SetAttribute(name, dictionary);

            return new DictionaryMediaNode(dictionary);
        }

        public void SetAttribute(string name, object value)
        {
            if (_values.ContainsKey(name))
            {
                _values[name] = value;
            }
            else
            {
                _values.Add(name, value);
            }
        }

        public void WriteLinks(IEnumerable<Link> links)
        {
            throw new NotSupportedException();
        }

        public IMediaNodeList AddList(string nodeName, string leafName)
        {
            var list = new DictionaryMediaNodeList();
            SetAttribute(nodeName, list.List);

            return list;
        }

        public static IDictionary<string, object> Write(Action<IMediaNode> configuration)
        {
            var node = new DictionaryMediaNode();
            configuration(node);

            return node.Values;
        }
    }
}