using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Registration;
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
            Exclude(x => x.FirstCall() != null && x.FirstCall().HasAttribute<DoNotCompressAttribute>());
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
                // This isn't something that changes very often at all
                var encodings = new IHttpContentEncoding[] { new GZipHttpContentEncoding(), new DeflateHttpContentEncoding() };
                var encoders = new HttpContentEncoders(encodings);
                services.SetServiceIfNone<IHttpContentEncoders>(encoders);
            });

            registry.Policies.Add(new ContentCompressionConvention(_filters.Matches));
        }

        // Instrumentation lets us go after the asset stuff
        [ConfigurationType(ConfigurationType.Instrumentation)]
        public class ContentCompressionConvention : IConfigurationAction
        {
            private readonly Func<BehaviorChain, bool> _predicate;

            public ContentCompressionConvention(Func<BehaviorChain, bool> predicate)
            {
                _predicate = predicate;
            }

            public void Configure(BehaviorGraph graph)
            {
                graph
                    .Behaviors
                    .Where(_predicate)
                    .Each(chain =>
                    {
                        var encoders = graph.Services.DefaultServiceFor<IHttpContentEncoders>().Value.As<IHttpContentEncoders>();
                        chain.Filters.Add(new HttpContentEncodingFilter(encoders));
                    });
            }
        }
    }
}