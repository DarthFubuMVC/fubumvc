using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using FubuCore.Logging;

namespace FubuMVC.Core.Diagnostics.Instrumentation
{
    // Break it out. This needs to be a singleton

    public class PerformanceHistoryQueue : IPerformanceHistoryQueue
    {
        private readonly ILogger _logger;

        public PerformanceHistoryQueue(ILogger logger)
        {
            _logger = logger;
        }


        public void Enqueue(ChainExecutionLog log)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    log.RootChain?.Performance?.Read(log);
                }
                catch (Exception ex)
                {
                    try
                    {
                        _logger.Info("Failed while updating performance history: " + ex);
                    }
                    catch (Exception e)
                    {
                        // Yeah, I know, but don't allow an exception here to *ever* bubble up
                        Console.WriteLine(e);
                    }
                }
            });

        }

    }
}