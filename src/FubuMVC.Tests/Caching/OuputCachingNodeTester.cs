using System;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Resources.Etags;
using NUnit.Framework;
using FubuTestingSupport;
using System.Linq;

namespace FubuMVC.Tests.Caching
{
    [TestFixture]
    public class OuputCachingNodeTester
    {
        private OutputCachingNode theNode;

        [SetUp]
        public void SetUp()
        {
            theNode = new OutputCachingNode();
        }

        [Test]
        public void by_default_uses_ResourceHash_with_vary_by_chain()
        {
            var def = toObjectDef();

            var resourceDef = def.DependencyFor<IResourceHash>().As<ConfiguredDependency>().Definition;

            resourceDef.ShouldBeTheSameAs(theNode.ResourceHash);

            resourceDef.Type.ShouldEqual(typeof (ResourceHash));

            resourceDef.EnumerableDependenciesOf<IVaryBy>().Items.Single().Type
                .ShouldEqual(typeof (VaryByResource));

        }

        [Test]
        public void apply_with_a_bad_type()
        {
            Exception<ArgumentException>.ShouldBeThrownBy(() =>
            {
                theNode.Apply(GetType());
            });
        }

        [Test]
        public void apply_adds_extra_vary_by()
        {
            theNode.Apply<VaryByThreadCulture>();

            theNode.ResourceHash.EnumerableDependenciesOf<IVaryBy>()
                .Items.Last().Type.ShouldEqual(typeof (VaryByThreadCulture));
        }


        [Test]
        public void apply_adds_extra_vary_by_2()
        {
            theNode.Apply(typeof(VaryByThreadCulture));

            theNode.ResourceHash.EnumerableDependenciesOf<IVaryBy>()
                .Items.Last().Type.ShouldEqual(typeof(VaryByThreadCulture));
        }

        private ObjectDef toObjectDef()
        {
            return theNode.As<IContainerModel>().ToObjectDef();
        }

        [Test]
        public void builds_the_output_caching_behavior()
        {
            toObjectDef().Type.ShouldEqual(typeof (OutputCachingBehavior));
        }

        [Test]
        public void the_category_is_caching()
        {
            theNode.Category.ShouldEqual(BehaviorCategory.Cache);
        }

        [Test]
        public void uses_default_etag_cache_if_none_specified()
        {
            toObjectDef().DependencyFor<IEtagCache>().ShouldBeNull();
        }

        [Test]
        public void uses_explicit_etag_cache_if_one_is_specified()
        {
            theNode.ETagCache = new ObjectDef(typeof(FakeEtagCache));

            toObjectDef().FindDependencyDefinitionFor(typeof (IEtagCache))
                .ShouldEqual(theNode.ETagCache);
                
        }

        [Test]
        public void uses_the_default_output_cache_if_not_specified()
        {
            toObjectDef().DependencyFor<IEtagCache>().ShouldBeNull();
        }

        [Test]
        public void uses_explicit_output_cache_if_one_is_specified()
        {
            theNode.OutputCache = ObjectDef.ForType<FakeOutputCache>();

            toObjectDef().FindDependencyDefinitionFor<IOutputCache>()
                .ShouldEqual(theNode.OutputCache);
        }
    }

    public class FakeOutputCache : IOutputCache
    {
        public IRecordedOutput Retrieve(string resourceHash, Func<IRecordedOutput> cacheMiss)
        {
            throw new NotImplementedException();
        }
    }

    public class FakeEtagCache : IEtagCache
    {
        public string Current(string resourceHash)
        {
            throw new NotImplementedException();
        }

        public void Register(string resourceHash, string etag)
        {
            throw new NotImplementedException();
        }

        public void Eject(string resourceHash)
        {
            throw new NotImplementedException();
        }
    }
}