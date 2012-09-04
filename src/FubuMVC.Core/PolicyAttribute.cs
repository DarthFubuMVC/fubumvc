namespace FubuMVC.Core
{
    public class PolicyAttribute : ConfigurationTypeAttribute
    {
        public PolicyAttribute()
            : base(ConfigurationType.Policy)
        {
        }
    }
}