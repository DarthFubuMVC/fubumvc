using FubuMVC.Core.Assets;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.Assets
{
    [TestFixture]
    public class AssetNamesKeyTester
    {
        [Test]
        public void equals()
        {
            var key1 = new AssetNamesKey(new string[]{"a", "b", "c"});
            var key2 = new AssetNamesKey(new string[]{"a", "b", "c"});
            var key3 = new AssetNamesKey(new string[]{"d", "b", "c"});

            key1.ShouldEqual(key2);
            key2.ShouldEqual(key1);

            key3.ShouldNotEqual(key1);
            key1.ShouldNotEqual(key3);
        }

        [Test]
        public void equals_is_not_order_dependent()
        {
            var key1 = new AssetNamesKey(new string[] { "a", "b", "c" });
            var key2 = new AssetNamesKey(new string[] { "b", "c", "a" });
            var key3 = new AssetNamesKey(new string[] { "d", "b", "c" });

            key1.ShouldEqual(key2);
            key2.ShouldEqual(key1);

            key3.ShouldNotEqual(key1);
            key1.ShouldNotEqual(key3);
        }

        [Test]
        public void hash_code_is_predictable()
        {
            var key1 = new AssetNamesKey(new string[] { "a", "b", "c" });
            var key2 = new AssetNamesKey(new string[] { "a", "b", "c" });

            key1.GetHashCode().ShouldEqual(key2.GetHashCode());
            key1.GetHashCode().ShouldEqual(key2.GetHashCode());
            key1.GetHashCode().ShouldEqual(key2.GetHashCode());
            key1.GetHashCode().ShouldEqual(key2.GetHashCode());
            key1.GetHashCode().ShouldEqual(key2.GetHashCode());
        }
    }
}