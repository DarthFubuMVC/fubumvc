using System.Collections.Generic;
using FubuCore.Reflection;

namespace FubuMVC.Core.Projections
{
    public class SelfProjectingValueProjector<TParent, T> : ISingleValueProjection<TParent> where T : class, IProjectMyself
    {
        private readonly Accessor _accessor;

        public SelfProjectingValueProjector(Accessor accessor)
        {
            _accessor = accessor;
            AttributeName = accessor.Name;
        }

        public void Write(IProjectionContext<TParent> context, IMediaNode node)
        {
            var value = context.Values.ValueFor(_accessor) as T;
            if (value != null)
            {
                value.Project(AttributeName, node);
            }
        }

        IEnumerable<Accessor> IProjection<TParent>.Accessors()
        {
            yield return _accessor;
        }

        public string AttributeName { get; set; }
    }
}