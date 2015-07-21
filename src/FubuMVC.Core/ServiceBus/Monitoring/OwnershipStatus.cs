namespace FubuMVC.Core.ServiceBus.Monitoring
{
    public enum OwnershipStatus
    {
        OwnershipActivated,
        Exception,
        AlreadyOwned,
        UnknownSubject,
        TimedOut
    }
}