using System;
using System.Collections.Generic;
using FubuCore.Descriptions;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Resources.Conneg;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class CacheAttributePolicyTester
    {
        [Test]
        public void applies_to_resource_type_marked_as_Cache()
        {
            var graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Actions.IncludeType<SomeEndpoint>();
            });

            var chain = graph.BehaviorFor<SomeEndpoint>(x => x.get_resource());

            var node = chain.OfType<OutputCachingNode>().Single();

            node.VaryByPolicies().Single().ShouldEqual(typeof (VaryByThreadCulture));
        }

        [Test]
        public void applies_to_resource_type_marked_as_Cache_on_writer_only_chains()
        {
            var graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Actions.IncludeClassesSuffixedWithController(); // Just making it not use SomeEndpoint

                x.Configure(g =>
                {
                    var chain = BehaviorChain.ForWriter(new CachedResourceWriter());
                    g.AddChain(chain);
                });
            });

            var chain2 = graph.Behaviors.Single(x => x.ResourceType() == typeof (CachedResource));

            var node = chain2.OfType<OutputCachingNode>().Single();

            node.VaryByPolicies().Single().ShouldEqual(typeof(VaryByThreadCulture));
        }
    }

    public class CachedResourceWriter : WriterNode
    {
        public override Type ResourceType
        {
            get { return typeof (CachedResource); }
        }

        protected override ObjectDef toWriterDef()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<string> Mimetypes
        {
            get { throw new NotImplementedException(); }
        }

        protected override void createDescription(Description description)
        {
            throw new NotImplementedException();
        }
    }

    public class SomeEndpoint
    {
        public CachedResource get_resource()
        {
            return new CachedResource();
        }
    }

    [Cache(VaryBy = new []{typeof(VaryByThreadCulture)})]
    public class CachedResource
    {
        
    }
}