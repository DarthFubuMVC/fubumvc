using System.Collections.Generic;
using FubuMVC.Core.Content;
using NUnit.Framework;
using Rhino.Mocks;
using FubuCore;

namespace FubuMVC.Tests.Content
{
    [TestFixture]
    public class ContentRegistryCacheTester
    {
        private List<IImageUrlResolver> resolvers;
        private ContentRegistryCache theCache;

        [SetUp]
        public void SetUp()
        {
            resolvers = new List<IImageUrlResolver>();
            for (int i = 0; i < 4; i++) resolvers.Add(MockRepository.GenerateMock<IImageUrlResolver>());

            theCache = new ContentRegistryCache(resolvers);
        }

        private void registerUrl(string imageName, string imageUrl, int index)
        {
            resolvers[index].Stub(x => x.UrlFor(imageName)).Return(imageUrl);
        }

        [Test]
        public void uses_the_default_path_if_no_resolver_knows_the_name()
        {
            // hit it multiple times
            theCache.ImageUrl("site.png").ShouldEqual("~/content/images/site.png".ToAbsoluteUrl());
            theCache.ImageUrl("site.png").ShouldEqual("~/content/images/site.png".ToAbsoluteUrl());
            theCache.ImageUrl("site.png").ShouldEqual("~/content/images/site.png".ToAbsoluteUrl());
        }

        [Test]
        public void uses_a_resolved_url_if_it_exists_1()
        {
            registerUrl("site.png", "~/_images/something/site.png", 0);
            theCache.ImageUrl("site.png").ShouldEqual("~/_images/something/site.png".ToAbsoluteUrl());
        }

        [Test]
        public void uses_a_resolved_url_if_it_exists_2()
        {
            registerUrl("site.png", "~/_images/something/site.png", 1);
            theCache.ImageUrl("site.png").ShouldEqual("~/_images/something/site.png".ToAbsoluteUrl());
        }

        [Test]
        public void uses_a_resolved_url_if_it_exists_3()
        {
            registerUrl("site.png", "~/_images/something/site.png", 2);
            theCache.ImageUrl("site.png").ShouldEqual("~/_images/something/site.png".ToAbsoluteUrl());
        }
    }
}