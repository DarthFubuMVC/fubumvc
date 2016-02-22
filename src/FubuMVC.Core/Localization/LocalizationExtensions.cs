namespace FubuMVC.Core.Localization
{
    public static class LocalizationExtensions
    {
        public static LocalizationKey ToLocalizationKey(this PropertyToken propertyInfo)
        {
            return new LocalizationKey(propertyInfo.PropertyName, propertyInfo.ParentTypeName);
        }

    }
}