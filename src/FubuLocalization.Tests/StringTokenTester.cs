using System.Collections.Generic;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuLocalization.Tests
{
    [TestFixture]
    public class StringTokenTester
    {
        [Test]
        public void from_type_just_uses_type_name()
        {
            var token = StringToken.FromType<StringTokenTester>();
            token.Key.ShouldEqual(GetType().Name);
            token.DefaultValue.ShouldEqual(GetType().Name);
        }

        [Test]
        public void from_type_just_uses_type_name_2()
        {
            var token = StringToken.FromType(GetType());
            token.Key.ShouldEqual(GetType().Name);
            token.DefaultValue.ShouldEqual(GetType().Name);
        }

        [Test]
        public void two_instances_with_the_same_key_should_equal_equal_each_other()
        {
            var x = buildCommonToken();
            var y = buildCommonToken();

            x.ShouldEqual(y);
        }

        [Test]
        public void two_instances_with_the_same_key_should_be_considered_the_same_for_hashing_purposes()
        {
            var x = buildCommonToken();
            var y = buildCommonToken();

            var dict = new Dictionary<StringToken, int> { { x, 0 } };

            dict.ContainsKey(y).ShouldBeTrue();
        }

        [Test]
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
                    .ShouldEqual(retVal);
            }
        }

        [Test]
        public void should_render_to_string_as_empty_when_condition_is_false()
        {
            buildCommonToken()
                .ToString(false)
                .ShouldEqual(string.Empty);
        }

        private StringToken buildCommonToken()
        {
            const string key = "test";
            return StringToken.FromKeyString(key, "default");
        }
    }
}