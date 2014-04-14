using System;
using System.Collections.Generic;
using FubuCore.Reflection;

namespace FubuMVC.Core.Projections
{
    public class SingleValueProjection<T> : ISingleValueProjection<T>
    {
        public string AttributeName { get; set; }
        public Func<IProjectionContext<T>, object> Source { get; private set; }

        public SingleValueProjection(string attributeName, Func<IProjectionContext<T>, object> source)
        {
            AttributeName = attributeName;
            Source = source;
        }

        public void Write(IProjectionContext<T> context, IMediaNode node)
        {
            var value = Source(context);
            node.SetAttribute(AttributeName, value);
        }

        public IEnumerable<Accessor> Accessors()
        {
            // It's indeterminate anyway
            yield break;
        }
    }
}