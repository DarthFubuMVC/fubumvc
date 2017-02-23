using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using FubuCore.Reflection;
using FubuMVC.Core.Localization;
using FubuMVC.Core.Localization.Basic;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Localization.Basic
{
    
    public class AssemblyScannerTester
    {
        private InMemoryLocalizationStorage theStorage;
        private AssemblyScanner theScanner;
        private CultureInfo theCulture;

        public AssemblyScannerTester()
        {
            theCulture = new CultureInfo("en-GB");
            theStorage = new InMemoryLocalizationStorage();
            theScanner = new AssemblyScanner(theStorage, theCulture);

            

        }

        [Fact]
        public void the_default_culture_should_be_en_US()
        {
            theScanner.DefaultCulture.ShouldBe(new CultureInfo("en-US"));
        }

        [Fact]
        public void scan_string_token_class_that_is_all_missing_and_for_the_default_culture()
        {
            theScanner.DefaultCulture = theCulture;

            theScanner.ScanStringTokenType(typeof(AssemblyScannerTokens));

            theStorage.RecordedMissingKeysFor(theCulture).ShouldHaveTheSameElementsAs(LocalString.ReadAllFrom(@"
                A=a
                B=b
                C=c
                D=d
"));
        }


        [Fact]
        public void scan_string_token_class_that_is_all_missing_for_a_different_culture_than_the_default()
        {
            theScanner.ScanStringTokenType(typeof(AssemblyScannerTokens));

            theStorage.RecordedMissingKeysFor(theCulture).ShouldHaveTheSameElementsAs(LocalString.ReadAllFrom(@"
                A=en-GB_A
                B=en-GB_B
                C=en-GB_C
                D=en-GB_D
"));
        }

        [Fact]
        public void scan_string_token_class_when_some_are_not_missing()
        {
            theStorage.Add(theCulture, "A", "a-something");
            theStorage.Add(theCulture, "C", "c-something");

            theScanner.DefaultCulture = theCulture;

            theScanner.ScanStringTokenType(typeof(AssemblyScannerTokens));

            theStorage.RecordedMissingKeysFor(theCulture).ShouldHaveTheSameElementsAs(LocalString.ReadAllFrom(@"
                B=b
                D=d
"));
        }

        [Fact]
        public void scan_using_a_localized_property_implementation()
        {
            theScanner.DefaultCulture = theCulture;

            theScanner.ScanProperties(new ScanningPolicy());

            theStorage.RecordedMissingKeysFor(theCulture).ShouldHaveTheSameElementsAs(LocalString.ReadAllFrom(@"
                FubuLocalization.Tests.Basic.AssemblyScannerTester+ScanningTarget:Name:Header=Name
                FubuLocalization.Tests.Basic.AssemblyScannerTester+ScanningTarget:Title:Header=Title
                FubuLocalization.Tests.Basic.AssemblyScannerTester+ScanningTarget:Color:Header=Color
                X=x
                Y=y
                Z=z
"));
        }



        public class ScanningPolicy : ILocalizedProperties
        {
            public IEnumerable<PropertyInfo> FindProperties()
            {
                yield return ReflectionHelper.GetProperty<ScanningTarget>(x => x.Name);
                yield return ReflectionHelper.GetProperty<ScanningTarget>(x => x.Title);
                yield return ReflectionHelper.GetProperty<ScanningTarget>(x => x.Color);
            }

            public IEnumerable<StringToken> FindTokens()
            {
                yield return StringToken.FromKeyString("X", "x");
                yield return StringToken.FromKeyString("Y", "y");
                yield return StringToken.FromKeyString("Z", "z");
            }
        }

        public class ScanningTarget
        {
            public string Name { get; set; }
            public string Title { get; set; }
            public string Color { get; set; }
        }
    }

    public class AssemblyScannerTokens : StringToken
    {
        protected AssemblyScannerTokens(string key, string defaultValue) : base(key, defaultValue)
        {
        }

        public static readonly StringToken A = FromKeyString("A", "a");
        public static readonly StringToken B = FromKeyString("B", "b");
        public static readonly StringToken C = FromKeyString("C", "c");
        public static readonly StringToken D = FromKeyString("D", "d");
    }
}