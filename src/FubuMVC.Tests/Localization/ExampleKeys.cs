using System;
using FubuCore;
using FubuMVC.Core.Localization;

namespace FubuMVC.Tests.Localization
{
    // SAMPLE: ExampleKeys
    
    public class ExampleKeys : StringToken
    {
        public static readonly ExampleKeys InvalidFormat = new ExampleKeys("Data is formatted incorrectly");
        public static readonly ExampleKeys Required = new ExampleKeys("Required Field");

        public ExampleKeys(string text)
            : base(null, text)
        {
        }
    }

    // ENDSAMPLE

    // SAMPLE: ExampleNamespacing

    public class ExampleNamespacing : StringToken
    {
        public static readonly ExampleNamespacing One = new ExampleNamespacing("One");
        public static readonly ExampleNamespacing Two = new ExampleNamespacing("Two");

        public ExampleNamespacing(string text)
            : base(null, text, namespaceByType: true)
        {
        }
    }

    // ENDSAMPLE

    public class ConventionalKeyUsage
    {
        // SAMPLE: FromKeyStringUsage
        public static void GenerateTheToken()
        {
            var tokenWithNoDefaultValue = StringToken.FromKeyString("Literally any string imaginable");
            var tokenWithDefaultValue = StringToken.FromKeyString("Some:Namespaced:Key", "The default value");
        }
        // ENDSAMPLE

        public interface ICustomTemplate { }

        // SAMPLE: ConventionalKeyUsage
        public static StringToken TemplateHeader<T>()
            where T : ICustomTemplate
        {
            var templateType = typeof (T);
            var defaultValue = breakUpPascalCasing(templateType.Name);
            var tokenKey = "Template:{0}:Header".ToFormat(templateType.FullName);

            return StringToken.FromKeyString(tokenKey, defaultValue);
        }
        // ENDSAMPLE

        private static string breakUpPascalCasing(string value)
        {
            throw new NotImplementedException();
        }
    }
}