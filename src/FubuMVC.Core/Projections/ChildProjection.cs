using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore.Reflection;

namespace FubuMVC.Core.Projections
{
    public class ChildProjection<TParent, TChild> : Projection<TChild>, IProjection<TParent>, IAccessorProjection where TChild : class
    {
        private readonly Accessor _accessor;
        private string _name;
        private readonly Func<IProjectionContext<TParent>, TChild> _source;

        public ChildProjection(string name, Func<IProjectionContext<TParent>, TChild> source, DisplayFormatting formatting)
            : base(formatting)
        {
            _source = source;
            _name = name;
        }

        public ChildProjection(Expression<Func<TParent, TChild>> expression, DisplayFormatting formatting)
            : base(formatting)
        {
            _accessor = ReflectionHelper.GetAccessor(expression);
            _source = c => c.Values.ValueFor(_accessor) as TChild;
            _name = _accessor.Name;
        }

        void IAccessorProjection.ApplyNaming(IAccessorNaming naming)
        {
            _name = naming.Name(_accessor);
            Naming = naming;
        }

        /// <summary>
        /// Replace the node name in the projected values for this nested child
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ChildProjection<TParent, TChild> Name(string name)
        {
            _name = name;
            return this;
        }

        /// <summary>
        /// Define a projection inline for the TChild object
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public ChildProjection<TParent, TChild> Configure(Action<Projection<TChild>> configuration)
        {
            configuration(this);
            return this;
        }

        /// <summary>
        /// Explicit mapping of the TChild objects into the media structure.  This is the anything goes, last resort option
        /// </summary>
        /// <param name="explicitWriting"></param>
        /// <returns></returns>
        public ChildProjection<TParent, TChild> With(Action<IProjectionContext<TChild>, IMediaNode> explicitWriting)
        {
            return Configure(x => x.WriteWith(explicitWriting));
        }

        /// <summary>
        /// Mix in a predefined projection for TChild.  Use this to either reuse a projection or to break up complex
        /// nested definitions
        /// </summary>
        /// <typeparam name="TProjection"></typeparam>
        /// <returns></returns>
        public ChildProjection<TParent, TChild> With<TProjection>() where TProjection : IProjection<TChild>, new()
        {
            Include<TProjection>();
            return this;
        }

        IEnumerable<Accessor> IProjection<TParent>.Accessors()
        {
            if (_accessor != null) yield return _accessor;
        }

        void IProjection<TParent>.Write(IProjectionContext<TParent> context, IMediaNode node)
        {
            var value = _source(context);
            if (value == null) return;

            var childNode = node.AddChild(_name);

            var childContext = context.ContextFor(value);

            write(childContext, childNode);
        }
    }
}