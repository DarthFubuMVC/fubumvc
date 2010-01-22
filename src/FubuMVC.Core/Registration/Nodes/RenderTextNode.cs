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
            def.Child(MimeType);
        }
    }
}