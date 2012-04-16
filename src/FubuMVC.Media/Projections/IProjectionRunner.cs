using System.Collections.Generic;
using FubuCore;

namespace FubuMVC.Core.Projections
{
    public interface IProjectionRunner
    {
        void Run<T>(IProjection<T> projection, T subject, IMediaNode node);
        void Run<T, TProjection>(T subject, IMediaNode node) where TProjection : IProjection<T>;
        void Run<T>(IProjection<T> projection, IValues<T> values, IMediaNode node);
        void Run<T, TProjection>(IValues<T> values, IMediaNode node) where TProjection : IProjection<T>;
    }

    public class ProjectionRunner : IProjectionRunner
    {
        private readonly IServiceLocator _services;

        public ProjectionRunner(IServiceLocator services)
        {
            _services = services;
        }

        public void Run<T>(IProjection<T> projection, T subject, IMediaNode node)
        {
            Run(projection, new SimpleValues<T>(subject), node);
        }

        public void Run<T, TProjection>(T subject, IMediaNode node) where TProjection : IProjection<T>
        {
            var projection = _services.GetInstance<TProjection>();
            Run(projection, subject, node);
        }

        public void Run<T>(IProjection<T> projection, IValues<T> values, IMediaNode node)
        {
            var context = new ProjectionContext<T>(_services, values);
            projection.Write(context, node);
        }

        public void Run<T, TProjection>(IValues<T> values, IMediaNode node) where TProjection : IProjection<T>
        {
            var projection = _services.GetInstance<TProjection>();
            Run(projection, values, node);
        }
    }

    public interface IProjectionRunner<T>
    {
        void Run(IProjection<T> projection, T subject, IMediaNode node);
        void Run<TProjection>(T subject, IMediaNode node) where TProjection : IProjection<T>;
        void Run(IProjection<T> projection, IValues<T> values, IMediaNode node);
        void Run<TProjection>(IValues<T> values, IMediaNode node) where TProjection : IProjection<T>;

        IDictionary<string, object> ProjectToJson(IProjection<T> projection, IValues<T> values);
        IDictionary<string, object> ProjectToJson<TProjection>(IValues<T> values) where TProjection : IProjection<T>;
        IDictionary<string, object> ProjectToJson<TProjection>(T subject) where TProjection : IProjection<T>;
    }

    public class ProjectionRunner<T> : IProjectionRunner<T>
    {
        private readonly IProjectionRunner _runner;

        public ProjectionRunner(IProjectionRunner runner)
        {
            _runner = runner;
        }

        public void Run(IProjection<T> projection, T subject, IMediaNode node)
        {
            _runner.Run(projection, subject, node);
        }

        public void Run<TProjection>(T subject, IMediaNode node) where TProjection : IProjection<T>
        {
            _runner.Run<T, TProjection>(subject, node);
        }

        public void Run(IProjection<T> projection, IValues<T> values, IMediaNode node)
        {
            _runner.Run(projection, values, node);
        }

        public void Run<TProjection>(IValues<T> values, IMediaNode node) where TProjection : IProjection<T>
        {
            _runner.Run<T, TProjection>(values, node);
        }

        public IDictionary<string, object> ProjectToJson(IProjection<T> projection, IValues<T> values)
        {
            return DictionaryMediaNode.Write(node => Run(projection, values, node));
        }

        public IDictionary<string, object> ProjectToJson<TProjection>(IValues<T> values) where TProjection : IProjection<T>
        {
            return DictionaryMediaNode.Write(node => Run<TProjection>(values, node));
        }

        public IDictionary<string, object> ProjectToJson<TProjection>(T subject) where TProjection : IProjection<T>
        {
            return ProjectToJson<TProjection>(new SimpleValues<T>(subject));
        }
    }

}