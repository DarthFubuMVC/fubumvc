using FubuMVC.Core.Caching;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.Caching
{
    [TestFixture]
    public class ResourceHashTester
    {
        [Test]
        public void resource_hash_is_predictable()
        {
            var vary1 = new SimpleVaryBy().With("a", "1").With("b", "2");
            var vary2 = new SimpleVaryBy().With("a", "1").With("b", "2");
            var vary3 = new SimpleVaryBy().With("c", "3").With("d", "4");
            var vary4 = new SimpleVaryBy().With("c", "3").With("d", "4");
            var vary5 = new SimpleVaryBy().With("e", "5").With("f", "6");

            ResourceHash.For(vary1).ShouldEqual(ResourceHash.For(vary2));
            ResourceHash.For(vary1, vary3).ShouldEqual(ResourceHash.For(vary2, vary4));

            ResourceHash.For(vary1, vary3).ShouldNotEqual(ResourceHash.For(vary1, vary5));
        }
    }

    
}