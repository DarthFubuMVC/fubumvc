using System;
using FubuCore;

namespace FubuMVC.Core.ServiceBus.Monitoring
{
    public class HealthMonitoringSettings
    {
        private bool _initial = true;

        public int Seed
        {
            set { Random = new Random(value * 1000); }
        }

        public Random Random = new Random(60000);

        public int MinSeconds = 30;
        public int MaxSeconds = 60;

        public TimeSpan TakeOwnershipMessageTimeout = 30.Seconds();
        public TimeSpan HealthCheckMessageTimeout = 30.Seconds();
        public TimeSpan DeactivationMessageTimeout = 30.Seconds();
        public TimeSpan TaskAvailabilityCheckTimeout = 10.Seconds();
        public TimeSpan TaskActivationTimeout = 30.Seconds();

        public double Interval
        {
            get
            {
                if (_initial)
                {
                    _initial = false;
                    return 100;
                }
                
                return Random.Next(MinSeconds, MaxSeconds) * 1000;
            }
        }
    }
}