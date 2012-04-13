using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Diagnostics
{
    public class NodeRemoved : NodeEvent
    {
        private readonly TracedNode _node;

        public NodeRemoved(TracedNode node)
        {
            _node = node;
        }

        public TracedNode Node
        {
            get { return _node; }
        }

        public bool Equals(NodeRemoved other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._node, _node);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (NodeRemoved)) return false;
            return Equals((NodeRemoved) obj);
        }

        public override int GetHashCode()
        {
            return (_node != null ? _node.GetHashCode() : 0);
        }
    }
}