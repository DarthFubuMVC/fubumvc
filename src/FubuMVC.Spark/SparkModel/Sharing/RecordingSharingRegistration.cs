using System;
using System.Collections.Generic;
using Bottles.Diagnostics;

namespace FubuMVC.Spark.SparkModel.Sharing
{
    // Should be possible to use in conjunction with DSL if we make composition happen in each configure in SparkEngine.

    public class RecordingSharingRegistration : ISharingRegistration, ISharingPolicy
    {
        private readonly IList<Action<ISharingRegistration>> _registrations = new List<Action<ISharingRegistration>>();
        private Action<ISharingRegistration> record
        {
            set { _registrations.Add(value); }
        }

        public void Apply(IPackageLog log, ISharingRegistration registration)
        {
            _registrations.Each(x => x(registration));
        }

        public void Global(string global)
        {
            record = x => x.Global(global);
        }

        public void Dependency(string dependent, string dependency)
        {
            record = x => x.Dependency(dependent, dependency);
        }
    }
}