using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Diagnostics
{
    public class NodeReplaced : NodeEvent
    {
        private readonly TracedNode _oldNode;
        private readonly TracedNode _newNode;

        public NodeReplaced(TracedNode oldNode, TracedNode newNode)
        {
            _oldNode = oldNode;
            _newNode = newNode;
        }

        public TracedNode OldNode
        {
            get { return _oldNode; }
        }

        public TracedNode NewNode
        {
            get { return _newNode; }
        }

        public bool Equals(NodeReplaced other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._oldNode, _oldNode) && Equals(other._newNode, _newNode);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (NodeReplaced)) return false;
            return Equals((NodeReplaced) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_oldNode != null ? _oldNode.GetHashCode() : 0)*397) ^ (_newNode != null ? _newNode.GetHashCode() : 0);
            }
        }
    }
}