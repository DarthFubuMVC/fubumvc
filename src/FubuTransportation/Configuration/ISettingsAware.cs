namespace FubuTransportation.Configuration
{
    public interface ISettingsAware
    {
        void ApplySettings(object settings, ChannelNode node);
    }
}