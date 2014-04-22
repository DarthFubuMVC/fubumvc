using System;
using System.Linq;
using System.Linq.Expressions;
using FubuMVC.Core.Http.Compression;
using FubuMVC.Core.Registration;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.Tests.Http.Compression
{
    [TestFixture]
    public class ContentCompressionExtensionTester
    {
        [Test]
        public void compresses_by_default()
        {
            var graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Actions.IncludeType<MixedCompressionController>();

                x.Policies.Local.Add(policy => policy.ContentCompression.Apply());
            });

            chainHasFilter<MixedCompressionController>(graph, x => x.get_compressed()).ShouldBeTrue();
        }

        [Test]
        public void excludes_class_level_do_not_compress_attribute()
        {
            var graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Actions.IncludeType<NoCompressionController>();
                x.Policies.Local.Add(policy => policy.ContentCompression.Apply());
            });

            chainHasFilter<NoCompressionController>(graph, x => x.get_stuff()).ShouldBeFalse();
        }

        [Test]
        public void mixing_and_matching_default_compression()
        {
            var graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Actions.IncludeType<MixedCompressionController>();
                x.Policies.Local.Add(policy => policy.ContentCompression.Apply());
            });

            chainHasFilter<MixedCompressionController>(graph, x => x.get_compressed()).ShouldBeTrue();
            chainHasFilter<MixedCompressionController>(graph, x => x.get_without_compression()).ShouldBeFalse();
        }

        private bool chainHasFilter<T>(BehaviorGraph graph, Expression<Func<T, object>> expression)
        {
            return graph
                .BehaviorFor(expression)
                .Filters
                .Any(x => x.GetType() == typeof (HttpContentEncodingFilter));
        }
    }

    [DoNotCompress]
    public class NoCompressionController
    {
        public HtmlDocument get_stuff()
        {
            return new HtmlDocument();
        }
    }

    public class MixedCompressionController
    {
        public HtmlDocument get_compressed()
        {
            return new HtmlDocument();
        }

        [DoNotCompress]
        public HtmlDocument get_without_compression()
        {
            return new HtmlDocument();
        }
    }
}