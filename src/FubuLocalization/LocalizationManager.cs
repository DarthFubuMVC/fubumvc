using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore.Reflection;

namespace FubuLocalization
{
    public static class LocalizationManager
    {
        private const string PluralSufix = "_PLURAL";

        private static Func<ILocalizationDataProvider> LocalizationDataProvider;

        public static ILocalizationDataProvider CurrentProvider()
        {
            return LocalizationDataProvider();
        }

        static LocalizationManager()
        {
            Stub();
        }

        public static string ToHeader(this PropertyInfo property)
        {
            return GetHeader(property);
        }

        public static string ToHeader(this Accessor property)
        {
            return GetHeader(property.InnerProperty);
        }

        public static void RegisterProvider(Func<ILocalizationDataProvider> provider)
        {
            LocalizationDataProvider = provider;
        }

        public static void Stub()
        {
            Stub("en-US");
        }
        
        public static void Stub(string culture)
        {
            Stub(new NulloLocalizationDataProvider(new CultureInfo(culture)));
        }

        public static void Stub(ILocalizationDataProvider dataProvider)
        {
            LocalizationDataProvider = () => dataProvider;
        }

        public static string GetTextForKey(StringToken token)
        {
            return LocalizationDataProvider().GetTextForKey(token);
        }

        public static string GetText(PropertyInfo property)
        {
            return LocalizationDataProvider().GetHeader(property);
        }

        public static string GetText<T>(Expression<Func<T, object>> expression)
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression);
            return GetHeader(propertyInfo);
        }

        public static LocalString GetLocalString(LambdaExpression expression)
        {
            var propertyInfo = ReflectionHelper.GetProperty(expression);
            return new LocalString { display = GetHeader(propertyInfo), value = propertyInfo.Name };
        }

        public static string GetText(Type type)
        {
            return LocalizationDataProvider().GetTextForKey(KeyFromType(type));
        }

        public static StringToken KeyFromType(Type type)
        {
            return StringToken.FromKeyString(type.Name);
        }

        public static string GetHeader(PropertyInfo property)
        {
            return LocalizationDataProvider().GetHeader(property);
        }

        public static string GetHeader(PropertyToken token)
        {
            return LocalizationDataProvider().GetHeader(token);
        }

        public static string GetHeader<T>(Expression<Func<T, object>> expression)
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression);
            return GetHeader(propertyInfo);
        }

        public static string GetTextForType(string name)
        {
            return LocalizationDataProvider().GetTextForKey(StringToken.FromKeyString(name));
        }

        public static string GetPluralTextForType(Type type)
        {
            return GetPluralTextForType(type.Name);
        }

        public static string GetPluralTextForType(string type)
        {
            return LocalizationDataProvider()
                    .GetTextForKey(StringToken.FromKeyString(type + PluralSufix));
        }
    }
}