using System;
using FubuCore;
using FubuCore.Descriptions;

namespace FubuMVC.Core.ServiceBus.Monitoring
{
    public class HealthMonitoringSettings : DescribesItself
    {
        private bool _initial = true;

        public void Describe(Description description)
        {
            description.Properties[nameof(MinSeconds)] = MinSeconds.ToString();
            description.Properties[nameof(MaxSeconds)] = MaxSeconds.ToString();
            description.Properties[nameof(TakeOwnershipMessageTimeout)] = TakeOwnershipMessageTimeout.ToString();
            description.Properties[nameof(HealthCheckMessageTimeout)] = HealthCheckMessageTimeout.ToString();
            description.Properties[nameof(DeactivationMessageTimeout)] = DeactivationMessageTimeout.ToString();
            description.Properties[nameof(TaskAvailabilityCheckTimeout)] = TaskAvailabilityCheckTimeout.ToString();
            description.Properties[nameof(TaskActivationTimeout)] = TaskActivationTimeout.ToString();
        }

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