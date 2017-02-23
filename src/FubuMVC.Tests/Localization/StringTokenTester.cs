using System.Collections.Generic;
using FubuMVC.Core.Localization;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.Localization
{
    
    public class StringTokenTester
    {
        [Fact]
        public void equals_is_namespace_aware()
        {
            var token1 = StringToken.FromKeyString("something");
            var token2 = StringToken.FromKeyString("something");
            var token3 = StringToken.FromKeyString("else");

            // FakeToken is namespaced
            var token4 = new FakeToken("something");
            var token5 = new FakeToken("something");
            var token6 = new FakeToken("else");

            token1.ShouldBe(token2);
            token2.ShouldBe(token1);
            token3.ShouldNotBe(token1);
            token1.ShouldNotBe(token3);

            token4.ShouldBe(token5);
            token5.ShouldBe(token4);
            token6.ShouldNotBe(token4);
            token4.ShouldNotBe(token6);

            // Namespace matters here
            token1.ShouldNotBe(token4);
            token3.ShouldNotBe(token6);
        }

        [Fact]
        public void GetHashCode_depends_on_the_localization_key()
        {
            var token1 = StringToken.FromKeyString("something");
            var token2 = StringToken.FromKeyString("something");
            var token3 = StringToken.FromKeyString("else");

            // FakeToken is namespaced
            var token4 = new FakeToken("something");
            var token5 = new FakeToken("something");
            var token6 = new FakeToken("else");

            token1.GetHashCode().ShouldBe(token2.GetHashCode());
            token2.GetHashCode().ShouldBe(token1.GetHashCode());
            token3.GetHashCode().ShouldNotBe(token1.GetHashCode());
            token1.GetHashCode().ShouldNotBe(token3.GetHashCode());

            token4.GetHashCode().ShouldBe(token5.GetHashCode());
            token5.GetHashCode().ShouldBe(token4.GetHashCode());
            token6.GetHashCode().ShouldNotBe(token4.GetHashCode());
            token4.GetHashCode().ShouldNotBe(token6.GetHashCode());

            // Namespace matters here
            token1.GetHashCode().ShouldNotBe(token4.GetHashCode());
            token3.GetHashCode().ShouldNotBe(token6.GetHashCode());
        }

        [Fact]
        public void from_type_just_uses_type_name()
        {
            var token = StringToken.FromType<StringTokenTester>();
            token.Key.ShouldBe(GetType().Name);
            token.DefaultValue.ShouldBe(GetType().Name);
        }

        [Fact]
        public void from_type_just_uses_type_name_2()
        {
            var token = StringToken.FromType(GetType());
            token.Key.ShouldBe(GetType().Name);
            token.DefaultValue.ShouldBe(GetType().Name);
        }

        [Fact]
        public void two_instances_with_the_same_key_should_equal_equal_each_other()
        {
            var x = buildCommonToken();
            var y = buildCommonToken();

            x.ShouldBe(y);
        }

        [Fact]
        public void two_instances_with_the_same_key_should_be_considered_the_same_for_hashing_purposes()
        {
            var x = buildCommonToken();
            var y = buildCommonToken();

            var dict = new Dictionary<StringToken, int> { { x, 0 } };

            dict.ContainsKey(y).ShouldBeTrue();
        }

        [Fact]
        public void should_render_to_string_from_localization_when_condition_is_true()
        {
            var mocks = new MockRepository();
            var provider = mocks.StrictMock<ILocalizationDataProvider>();
            var token = buildCommonToken();
            const string retVal = "TheText";

            LocalizationManager.Stub(provider);

            using (mocks.Record())
            {
                Expect.Call(provider.GetTextForKey(token)).Return(retVal);
            }

            using (mocks.Playback())
            {
                token
                    .ToString(true)
                    .ShouldBe(retVal);
            }
        }

        [Fact]
        public void should_render_to_string_as_empty_when_condition_is_false()
        {
            buildCommonToken()
                .ToString(false)
                .ShouldBe(string.Empty);
        }

        [Fact]
        public void should_implicitly_convert_to_string()
        {
            var mocks = new MockRepository();
            var provider = mocks.StrictMock<ILocalizationDataProvider>();
            var token = buildCommonToken();
            const string retVal = "TheText";

            LocalizationManager.Stub(provider);

            using (mocks.Record())
            {
                Expect.Call(provider.GetTextForKey(token)).Return(retVal);
            }

            using (mocks.Playback())
            {
                string result = token;
                result.ShouldBe(retVal);
            }
        }

        private StringToken buildCommonToken()
        {
            const string key = "test";
            return StringToken.FromKeyString(key, "default");
        }

        [Fact]
        public void find_by_type()
        {
            StringToken.Find(typeof (TargetKey), "One").ShouldBeTheSameAs(TargetKey.One);
            StringToken.Find(typeof (TargetKey2), "One").ShouldBeTheSameAs(TargetKey2.One);
        }

    }

    public class TargetKey : StringToken
    {
        public static readonly TargetKey One = new TargetKey("One");
        public static readonly TargetKey Two = new TargetKey("Two");

        protected TargetKey(string defaultValue) : base(null, defaultValue, namespaceByType:true)
        {
        }
    }

    public class TargetKey2 : StringToken
    {
        protected TargetKey2(string defaultValue)
            : base(null, defaultValue, namespaceByType: true)
        {
        }

        public static readonly TargetKey2 One = new TargetKey2("One");
        public static readonly TargetKey2 Two = new TargetKey2("Two");
    }

    public class FakeToken : StringToken
    {
        public FakeToken(string key, string defaultValue = null) : base(key, defaultValue, namespaceByType:true)
        {
        }
    }
}