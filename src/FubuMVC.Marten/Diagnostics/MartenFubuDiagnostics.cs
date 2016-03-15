using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.Diagnostics.Instrumentation;

namespace FubuMVC.Marten.Diagnostics
{
    public class MartenFubuDiagnostics
    {
        private readonly IChainExecutionHistory _history;

        public MartenFubuDiagnostics(IChainExecutionHistory history)
        {
            _history = history;
        }

        public Dictionary<string, object>[] get_sessions()
        {
            return _history.RecentReports().Where(x => x.RootChain != null).Select(report =>
            {
                var dict = new Dictionary<string, object>();

                dict.Add("chain", report.RootChain.Title());
                dict.Add("time", report.Time);
                dict.Add("hash", report.RootChain.Title().GetHashCode());
                dict.Add("execution_time", report.ExecutionTime);
                dict.Add("request", report.Id);

                var requestCount = report.Steps.Count(x => x.Log is CommandExecuted);

                dict.Add("request_count", requestCount);


                return dict;
            }).ToArray();
        }

        public Dictionary<string, object>[] get_session_Id(RequestCommands request)
        {
            var report = _history.Find(request.Id);
            if (report == null) return new Dictionary<string, object>[0];

            return report.Steps.Select(x => x.Log).OfType<CommandExecuted>().Select(cmd =>
            {
                var dict = new Dictionary<string, object> {{"sql", cmd.CommandText}, {"args", cmd.Args}};


                if (cmd is CommandFailed)
                {
                    dict.Add("success", false);
                    dict.Add("error", cmd.As<CommandFailed>().ExceptionText);
                }
                else
                {
                    dict.Add("success", true);
                }

                return dict;
            }).ToArray();

        } 

        
    }

    public class RequestCommands
    {
        public Guid Id { get; set; }
    }
}