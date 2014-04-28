using System.Collections.Generic;
using FubuCore.Reflection;

namespace FubuMVC.Core.Projections
{
    /// <summary>
    /// Used internally.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TProjection"></typeparam>
    public class DelegatingProjection<T, TProjection> : IProjection<T> where TProjection : IProjection<T>, new()
    {
        public void Write(IProjectionContext<T> context, IMediaNode node)
        {
            new TProjection().Write(context, node);
        }

        public IEnumerable<Accessor> Accessors()
        {
            return new TProjection().Accessors();
        }
    }
}