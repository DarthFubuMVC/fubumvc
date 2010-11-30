using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace FubuMVC.Core.Json
{
    public class ProjectionFactory : IProjectionFactory
    {
        private readonly IEnumerable<IProjectionSource> _sources;

        public ProjectionFactory(IEnumerable<IProjectionSource> sources)
        {
            _sources = sources;
        }

        public IProjection ProjectionFor(Type type)
        {
            var builder = typeof (Builder<>).CloseAndBuildAs<IBuilder>(type);
            return builder.Build(_sources);
        }

        public IProjection ProjectionFor(Type type, string name)
        {
            var builder = typeof (Builder<>).CloseAndBuildAs<IBuilder>(type);
            return builder.Build(_sources, name);
        }

        #region Nested type: Builder

        public class Builder<T> : IBuilder
        {
            public IProjection Build(IEnumerable<IProjectionSource> sources)
            {
                var values = sources.SelectMany(x => x.ValueProjectionsFor<T>());
                return new Projection<T>(values);
            }

            public IProjection Build(IEnumerable<IProjectionSource> sources, string name)
            {
                var values = sources.SelectMany(x => x.ValueProjectionsFor<T>(name));
                return new Projection<T>(values);
            }
        }

        #endregion

        #region Nested type: IBuilder

        public interface IBuilder
        {
            IProjection Build(IEnumerable<IProjectionSource> sources);
            IProjection Build(IEnumerable<IProjectionSource> sources, string name);
        }

        #endregion
    }
}