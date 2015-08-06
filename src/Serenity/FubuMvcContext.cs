using System;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Diagnostics.Runtime;
using StoryTeller;
using StoryTeller.Engine;

namespace Serenity
{
    public class FubuMvcContext : IExecutionContext
    {
        private readonly FubuMvcSystem _system;
        private readonly string _sessionTag = Guid.NewGuid().ToString();

        public FubuMvcContext(FubuMvcSystem system)
        {
            _system = system;
        }

        public void Dispose()
        {
        }


        public T GetService<T>()
        {
            throw new Exception();
            //return _system.Application.Services.GetInstance<T>();
        }

        public virtual void AfterExecution(ISpecContext context)
        {
            /*
            var reporter = new RequestReporter(_system);
            var requestLogs = Services.GetInstance<IRequestHistoryCache>().RecentReports().Where(x => x.SessionTag == _sessionTag).ToArray();
            reporter.Append(requestLogs);

            context.Reporting.Log(reporter);

            _system.ApplyLogging(context);
             */
        }

        public void BeforeExecution(ISpecContext context)
        {
            throw new Exception("no");
        }
    }
}