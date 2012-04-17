using System;

namespace FubuMVC.Media.Projections
{
    public interface IProjection<T>
    {
        void Write(IProjectionContext<T> context, IMediaNode node);
    }

    public class LambdaProjection<T> : IProjection<T>
    {
        private readonly Action<IProjectionContext<T>, IMediaNode> _writer;

        public LambdaProjection(Action<IProjectionContext<T>, IMediaNode> writer)
        {
            _writer = writer;
        }

        public void Write(IProjectionContext<T> context, IMediaNode node)
        {
            _writer(context, node);
        }
    }
}