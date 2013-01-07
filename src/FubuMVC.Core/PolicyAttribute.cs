namespace FubuMVC.Core
{
    /// <summary>
    /// Equivalent to [ConfigurationType(ConfigurationType.Policy)]
    /// </summary>
    public class PolicyAttribute : ConfigurationTypeAttribute
    {
        public PolicyAttribute()
            : base(ConfigurationType.Policy)
        {
        }
    }
}