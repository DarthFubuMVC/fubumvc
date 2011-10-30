namespace FubuMVC.Core.Assets.Caching
{
    public class AssetFileMonitoringSettings
    {
        // TODO -- make this editable somewhere
        public AssetFileMonitoringSettings()
        {
            MonitoringIntervalTime = 5000;
        }

        public double MonitoringIntervalTime { get; set; }
    }
}