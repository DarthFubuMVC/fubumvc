using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Diagnostics
{
    public interface IConfigurationObserver
    {
        bool IsRecording { get; }
        void RecordCallStatus(ActionCall call, string status);
        IEnumerable<string> GetLog(ActionCall call);
        IEnumerable<ActionCall> RecordedCalls();
    }

    public class NulloConfigurationObserver : IConfigurationObserver
    {
        public bool IsRecording
        {
            get { return false; }
        }

        public void RecordCallStatus(ActionCall call, string status)
        {
            // no-op
        }

        public void RecordStatus(string status)
        {
            // no-op
        }

        public IEnumerable<string> GetLog(ActionCall call)
        {
            // no-op
            yield break;
        }

        public IEnumerable<ActionCall> RecordedCalls()
        {
            // no-op
            yield break;
        }
    }

    public class RecordingConfigurationObserver : IConfigurationObserver
    {
        private readonly Cache<ActionCall, IList<string>> _log = new Cache<ActionCall, IList<string>>(c => new List<string>());

        public bool IsRecording
        {
            get { return true; }
        }

        public void RecordCallStatus(ActionCall call, string description)
        {
            _log[call].Add(description);
            LastLogEntry = description;
        }

        public IEnumerable<string> GetLog(ActionCall call)
        {
            return _log[call];
        }

        public IEnumerable<ActionCall> RecordedCalls()
        {
            return _log.GetAllKeys();
        }

        public string LastLogEntry { get; private set; }
    }
}