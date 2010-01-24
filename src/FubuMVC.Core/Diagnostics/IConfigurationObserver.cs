using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Util;

namespace FubuMVC.Core.Diagnostics
{
    public interface IConfigurationObserver
    {
        void RecordCallModification(ActionCall call, string status);
        //void RecordStatus(string status);
        IEnumerable<string> GetLog(ActionCall call);
    }

    public class NulloConfigurationObserver : IConfigurationObserver
    {
        public void RecordCallModification(ActionCall call, string status)
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
    }

    public class RecordingConfigurationObserver : IConfigurationObserver
    {
        private readonly Cache<ActionCall, IList<string>> _log = new Cache<ActionCall, IList<string>>(c=>new List<string>());

        public void RecordCallModification(ActionCall call, string description)
        {
            _log[call].Add(description);
        }


        public IEnumerable<string> GetLog(ActionCall call)
        {
            return _log[call];
        }
    }
}