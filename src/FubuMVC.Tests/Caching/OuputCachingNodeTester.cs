using System;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Resources.Etags;
using NUnit.Framework;
using FubuTestingSupport;

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