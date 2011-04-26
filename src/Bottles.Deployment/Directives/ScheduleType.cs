namespace Bottles.Deployment.Directives
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