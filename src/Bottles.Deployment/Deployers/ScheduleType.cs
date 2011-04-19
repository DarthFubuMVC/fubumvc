namespace Bottles.Deployment.Deployers
{
    public enum ScheduleType
    {
        Minute,
        Hourly,
        Daily,
        Weekly,
        Monthly,
        Once,
        OnStart,
        OnLogon,
        OnIdle
    }
}