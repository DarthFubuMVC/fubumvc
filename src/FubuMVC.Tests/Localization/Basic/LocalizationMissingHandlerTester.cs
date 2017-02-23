using System;
using System.Globalization;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Localization;
using FubuMVC.Core.Localization.Basic;
using FubuMVC.Tests.TestSupport;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.Localization.Basic
{
    
    public class LocalizationMissingHandlerTester : InteractionContext<LocalizationMissingHandler>
    {
        private CultureInfo theDefaultCulture = new CultureInfo("en-US");

        protected override void beforeEach()
        {
            Services.Inject(theDefaultCulture);
        }

        [Fact]
        public void find_missing_text_if_the_key_has_default_text_and_it_is_the_default_culture()
        {
            var token = StringToken.FromKeyString("KEY1", "the default text");
            
            ClassUnderTest.FindMissingText(token, theDefaultCulture).ShouldBe("the default text");

            MockFor<ILocalizationStorage>().AssertWasCalled(x => x.WriteMissing("KEY1", "the default text", theDefaultCulture));
        }

        [Fact]
        public void find_missing_text_if_the_key_does_not_have_default_text_and_it_is_the_default_culture()
        {
            var token = StringToken.FromKeyString("KEY1");

            ClassUnderTest.FindMissingText(token, theDefaultCulture).ShouldBe("en-US_KEY1");

            MockFor<ILocalizationStorage>().AssertWasCalled(x => x.WriteMissing("KEY1", "en-US_KEY1", theDefaultCulture));
        }

        [Fact]
        public void find_missing_text_if_the_key_has_default_text_but_it_is_not_the_default_culture()
        {
            var token = StringToken.FromKeyString("KEY1", "the default text");

            var cultureInfo = new CultureInfo("fr-FR");
            ClassUnderTest.FindMissingText(token, cultureInfo)
                .ShouldBe("fr-FR_KEY1");

            MockFor<ILocalizationStorage>().AssertWasCalled(x => x.WriteMissing("KEY1", "fr-FR_KEY1", cultureInfo));
        }

        [Fact]
        public void find_missing_text_if_the_key_has_default_text_but_it_is_not_the_default_culture_for_a_token_with_namespace()
        {
            var token = new FakeToken("KEY1", "the default text");

            var cultureInfo = new CultureInfo("fr-FR");
            ClassUnderTest.FindMissingText(token, cultureInfo)
                .ShouldBe("fr-FR_FakeToken:KEY1");

            MockFor<ILocalizationStorage>().AssertWasCalled(x => x.WriteMissing("FakeToken:KEY1", "fr-FR_FakeToken:KEY1", cultureInfo));
        }

        private string findMissingProperty(Expression<Func<MissingHandlerTarget, object>> expression, CultureInfo culture)
        {
            var propertyInfo = ReflectionHelper.GetProperty(expression);
            return ClassUnderTest.FindMissingProperty(new PropertyToken(propertyInfo), culture);
        }

        [Fact]
        public void find_missing_property_with_no_header_attribute_and_the_default_culture()
        {
            findMissingProperty(x => x.Name, theDefaultCulture).ShouldBe("Name");
        }

        [Fact]
        public void find_missing_property_with_no_header_attribute_and_non_default_culture()
        {
            findMissingProperty(x => x.Name, new CultureInfo("fr-FR")).ShouldBe("fr-FR_Name");
        }

        [Fact]
        public void find_missing_property_with_header_attribute_that_is_not_marked_by_culture_and_default_culture()
        {
            findMissingProperty(x => x.Title, theDefaultCulture).ShouldBe("The title");
        }

        [Fact]
        public void find_missing_property_with_header_attribute_that_is_not_marked_by_culture_and_a_non_default_culture()
        {
            findMissingProperty(x => x.Title, new CultureInfo("fr-FR")).ShouldBe("fr-FR_Title");
        }

        [Fact]
        public void find_missing_property_with_header_attribute_that_matches_the_culture()
        {
            findMissingProperty(x => x.AnotherTitle, new CultureInfo("en-GB")).ShouldBe("Other title");
        }

        [Fact]
        public void find_missing_property_with_header_attribute_that_does_not_match_the_culture()
        {
            findMissingProperty(x => x.AnotherTitle, theDefaultCulture).ShouldBe("Another Title");
        }

        [Fact]
        public void should_write_the_missing_property_value_to_storage()
        {
            var defaultValue = findMissingProperty(x => x.Name, theDefaultCulture);


            var key = "{0}:Name:Header".ToFormat(typeof (MissingHandlerTarget).FullName);
        
            MockFor<ILocalizationStorage>().AssertWasCalled(x => x.WriteMissing(key, defaultValue, theDefaultCulture));
        }
    }

    public class MissingHandlerTarget
    {
        public string Name { get; set; }

        [HeaderText("The title")]
        public string Title { get; set; }

        [HeaderText("Other title", Culture = "en-GB")]
        public string AnotherTitle { get; set;}
    }
}