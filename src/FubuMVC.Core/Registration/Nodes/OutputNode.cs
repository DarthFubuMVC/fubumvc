using System;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.ObjectGraph;
using HtmlTags;

namespace FubuMVC.Core.Registration.Nodes
{
    public class OutputNode : BehaviorNode
    {
        private readonly Type _behaviorType;

        public OutputNode(Type behaviorType)
        {
            _behaviorType = behaviorType;
            // TODO -- blow up if not an IActionBehavior type, must be concrete
        }

        public Type BehaviorType { get { return _behaviorType; } }

        public override BehaviorCategory Category { get { return BehaviorCategory.Output; } }
        public virtual string Description { get { return _behaviorType.Name; } }

        public static OutputNode For<T>() where T : IActionBehavior
        {
            return new OutputNode(typeof (T));
        }

        protected override sealed ObjectDef buildObjectDef()
        {
            var def = new ObjectDef(_behaviorType);
            configureObject(def);

            return def;
        }

        protected virtual void configureObject(ObjectDef def)
        {
        }

        public override string ToString()
        {
            return Description;
        }
    }

    public abstract class OutputNode<T> : OutputNode where T : IActionBehavior
    {
        protected OutputNode()
            : base(typeof (T))
        {
        }
    }

    public class RenderHtmlTagNode : OutputNode<RenderHtmlBehavior<HtmlTag>>
    {
        public override string Description { get { return "Write HtmlTag"; } }
    }

    public class RenderHtmlDocumentNode : OutputNode<RenderHtmlBehavior<HtmlDocument>>
    {
        public override string Description { get { return "Write HtmlDocument"; } }
    }
}