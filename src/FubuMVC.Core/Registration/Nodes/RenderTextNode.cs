using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Registration.Nodes
{
    public class RenderTextNode<T> : OutputNode where T : class
    {
        public RenderTextNode()
            : base(typeof (RenderTextBehavior<T>))
        {
            MimeType = MimeType.Text;
        }

        public MimeType MimeType { get; set; }

        public override string Description { get { return "Text as " + MimeType; } }

        protected override void configureObject(ObjectDef def)
        {
            def.DependencyByValue(MimeType);
        }

        public bool Equals(RenderTextNode<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.MimeType, MimeType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (RenderTextNode<T>)) return false;
            return Equals((RenderTextNode<T>) obj);
        }

        public override int GetHashCode()
        {
            return (MimeType != null ? MimeType.GetHashCode() : 0);
        }
    }
}