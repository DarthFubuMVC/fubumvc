using System.Collections.Generic;
using FubuCore.Reflection;

namespace FubuMVC.Core.Projections
{
    /// <summary>
    /// Used internally
    /// </summary>
    /// <typeparam name="TParent"></typeparam>
    /// <typeparam name="T"></typeparam>
    public class ExternallyFormattedValueProjector<TParent, T> : ISingleValueProjection<TParent>
    {
        private readonly Accessor _accessor;
        private readonly IValueProjector<T> _projector;

        public ExternallyFormattedValueProjector(Accessor accessor, IValueProjector<T> projector)
        {
            _accessor = accessor;
            _projector = projector;
            AttributeName = _accessor.Name;
        }

        public void Write(IProjectionContext<TParent> context, IMediaNode node)
        {
            var value = context.Values.ValueFor(_accessor);
            if (value != null)
            {
                _projector.Project(AttributeName, (T)value, node);
            }
        }

        public string AttributeName { get; set; }

        IEnumerable<Accessor> IProjection<TParent>.Accessors()
        {
            yield return _accessor;
        }
    }
}