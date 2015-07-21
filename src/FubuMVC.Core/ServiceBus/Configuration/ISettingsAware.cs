namespace FubuMVC.Core.ServiceBus.Configuration
{
    public interface ISettingsAware
    {
        void ApplySettings(object settings, ChannelNode node);
    }
}