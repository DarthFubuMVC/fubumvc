using System;

namespace FubuMVC.Core.Resources.Media.Projections
{
    public class SingleValueProjection<T> : IValueProjection<T>
    {
        public SingleValueProjection(string attributeName, Func<IProjectionContext<T>, object> source)
        {
            this.attributeName = attributeName;
            this.source = source;
        }

        protected string attributeName { get; set; }

        protected Func<IProjectionContext<T>, object> source { get; set; }

        public void WriteValue(IProjectionContext<T> target, IMediaNode node)
        {
            node.SetAttribute(attributeName, source(target));
        }
    }
}