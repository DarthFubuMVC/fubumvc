using FubuMVC.Core.Localization;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Localization
{
    [TestFixture]
    public class StringTokenToLocalizationKeyTester
    {
        [Test]
        public void localization_key_includes_the_namespace_if_it_exists()
        {
            NoNamespaceStringToken.Key1.ToLocalizationKey().Key1.ShouldBe("Key1");
        }

        [Test]
        public void localization_key_with_namespace()
        {
            StringTokenWithNamespace.Key1.ToLocalizationKey().Key1.ShouldBe("NS1:Key1");
        }
    }

    public class NoNamespaceStringToken : StringToken
    {
        public static readonly StringToken Key1 = new NoNamespaceStringToken("Key1", "something");

        protected NoNamespaceStringToken(string key, string defaultValue) : base(key, defaultValue)
        {

        }
    }

    public class StringTokenWithNamespace : StringToken
    {
        public static readonly StringToken Key1 = new StringTokenWithNamespace("Key1", "something");

        protected StringTokenWithNamespace(string key, string defaultValue) : base(key, defaultValue, "NS1")
        {

        }
    }
}