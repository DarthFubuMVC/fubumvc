using FubuCore;

namespace FubuMVC.Spark.SparkModel.Sharing
{
    public class SharingRegistrationDiagnostics : ISharingRegistration
    {
        private string _provenance = string.Empty;
        private readonly ISharingRegistration _inner;
        private readonly SharingLogsCache _logs;

        public SharingRegistrationDiagnostics(ISharingRegistration inner, SharingLogsCache logs)
        {
            _inner = inner;
            _logs = logs;
        }

        public void Global(string global)
        {
            _logs.FindByName(global)
                .Add(_provenance, "acts as global");

            _inner.Global(global);
        }

        public void Dependency(string dependent, string dependency)
        {
            _logs.FindByName(dependent)
                .Add(_provenance, "requires {0}".ToFormat(dependency));

            _logs.FindByName(dependency)
                .Add(_provenance, "is required by {0}".ToFormat(dependent));

            _inner.Dependency(dependent, dependency);
        }

        public void SetCurrentProvenance(string provenance)
        {
            _provenance = provenance;
        }

        public string CurrentProvenance
        {
            get { return _provenance; }
        }
    }
}