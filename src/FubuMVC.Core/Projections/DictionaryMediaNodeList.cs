using System.Collections.Generic;

namespace FubuMVC.Core.Projections
{
    public class DictionaryMediaNodeList : IMediaNodeList
    {
        private readonly IList<IDictionary<string, object>> _list;

        public DictionaryMediaNodeList() : this(new List<IDictionary<string, object>>())
        {
        }

        public DictionaryMediaNodeList(IList<IDictionary<string, object>> list)
        {
            _list = list;
        }

        public IList<IDictionary<string, object>> List
        {
            get { return _list; }
        }

        public IMediaNode Add()
        {
            var node = new DictionaryMediaNode();
            _list.Add(node.Values);

            return node;
        }
    }
}