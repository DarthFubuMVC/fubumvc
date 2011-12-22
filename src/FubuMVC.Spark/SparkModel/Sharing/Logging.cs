using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuCore;
using FubuCore.Util;

namespace FubuMVC.Spark.SparkModel.Sharing
{
    [DebuggerDisplay("{Provenance}: {Message}")]
    public class SharingLogEntry
    {
        public string Provenance { get; set; }
        public string Message { get; set; }
    }

    [DebuggerDisplay("Log For: {Name}: {System.Linq.Enumerable.Count(Logs)} entries")]
    public class SharingLog
    {
        private readonly IList<SharingLogEntry> _logs; 
        public SharingLog(string name)
        {
            Name = name;
            _logs = new List<SharingLogEntry>();
        }

        public string Name { get; private set; }
        public IEnumerable<SharingLogEntry> Logs
        {
            get { return _logs; }
        }

        public void Add(string provenance, string message)
        {
            _logs.Add(new SharingLogEntry
            {
                Provenance = provenance, 
                Message = message
            });
        }
    }

    public class SharingLogsCache
    {
        private readonly Cache<string, SharingLog> _logs = new Cache<string, SharingLog>();
        public SharingLogsCache()
        {
            _logs.OnMissing = n => new SharingLog(n.Capitalize());
        }

        public IEnumerable<SharingLog> Entries
        {
            get { return _logs.ToList(); }
        }

        public SharingLog FindByName(string name)
        {
            return _logs[name.Capitalize()];
        }
    }
}