using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Http.Compression
{
    public class ContentCompression : IFubuRegistryExtension
    {
        private readonly CompositeFilter<BehaviorChain> _filters;

        public ContentCompression()
        {
            _filters = new CompositeFilter<BehaviorChain>();
            Include(x => true);
        }
 
        public ContentCompression Include(Expression<Func<BehaviorChain, bool>> predicate)
        {
            _filters.Includes.Add(predicate);
            return this;
        }

        public ContentCompression Exclude(Expression<Func<BehaviorChain, bool>> predicate)
        {
            _filters.Excludes.Add(predicate);
            return this;
        }

        void IFubuRegistryExtension.Configure(FubuRegistry registry)
        {
            registry.Services(services =>
            {
                var encodings = new[] { new GZipHttpContentEncoding() };
                var encoders = new HttpContentEncoders(encodings);
                services.SetServiceIfNone<IHttpContentEncoders>(encoders);
            });

            registry.Configure(graph => graph.Behaviors.Where(_filters.Matches).Each(x =>
            {
                var encoders = graph.Services.DefaultServiceFor<IHttpContentEncoders>().Value.As<IHttpContentEncoders>();
                x.Filters.Add(new HttpContentEncodingFilter(encoders));
            }));
        }
    }
}