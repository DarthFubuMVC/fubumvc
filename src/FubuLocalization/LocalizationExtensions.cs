namespace FubuLocalization
{
    public static class LocalizationExtensions
    {
        public static LocalizationKey ToLocalizationKey(this PropertyToken propertyInfo)
        {
            return new LocalizationKey(propertyInfo.PropertyName, propertyInfo.ParentTypeName);
        }

        public static LocalizationKey ToLocalizationKey(this StringToken stringToken)
        {
            return new LocalizationKey(stringToken.Key);
        }
    }
}