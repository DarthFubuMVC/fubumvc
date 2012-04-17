using System;

namespace FubuMVC.Media.Projections
{
    public class SingleValueProjection<T> : IProjection<T>
    {
        public SingleValueProjection(string attributeName, Func<IProjectionContext<T>, object> source)
        {
            this.attributeName = attributeName;
            this.source = source;
        }

        protected string attributeName { get; set; }

        protected Func<IProjectionContext<T>, object> source { get; set; }

        public void Write(IProjectionContext<T> context, IMediaNode node)
        {
            node.SetAttribute(attributeName, source(context));
        }
    }
}