using System;
using System.Linq.Expressions;
using FubuCore.Reflection;

namespace FubuMVC.Media.Projections
{
    public class ChildProjection<TParent, TChild> : Projection<TChild>, IProjection<TParent> where TChild : class
    {
        private readonly Accessor _accessor;
        private string _name;

        public ChildProjection(Expression<Func<TParent, TChild>> expression, DisplayFormatting formatting) : base(formatting)
        {
            _accessor = ReflectionHelper.GetAccessor(expression);
            _name = _accessor.Name;
        }

        public ChildProjection<TParent, TChild> Name(string name)
        {
            _name = name;
            return this;
        }

        public ChildProjection<TParent, TChild> Configure(Action<Projection<TChild>> configuration)
        {
            configuration(this);
            return this;
        }

        public ChildProjection<TParent, TChild> With(Action<IProjectionContext<TChild>, IMediaNode> explicitWriting)
        {
            return Configure(x => x.WriteWith(explicitWriting));
        }

        public ChildProjection<TParent, TChild> With<TProjection>() where TProjection : IProjection<TChild>, new()
        {
            Include<TProjection>();
            return this;
        }

        void IProjection<TParent>.Write(IProjectionContext<TParent> context, IMediaNode node)
        {
            var value = context.ValueFor(_accessor) as TChild;
            if (value == null) return;

            var childNode = node.AddChild(_name);

            var childContext = context.ContextFor(value);

            write(childContext, childNode);
        }
    }
}