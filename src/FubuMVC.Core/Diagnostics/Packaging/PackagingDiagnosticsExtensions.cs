using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FubuMVC.Core.Diagnostics.Packaging
{
    public static class PackagingDiagnosticsExtensions
    {
        public static void LogExecutionOnEach<TItem>(this IActivationDiagnostics diagnostics, IEnumerable<TItem> targets, Action<TItem, IActivationLog> continuation)
        {
            targets.Each(currentTarget =>
            {
                var log = diagnostics.LogFor(currentTarget);
                diagnostics.LogExecution(currentTarget, () => continuation(currentTarget, log));
            });
        }

        public static void LogExecutionOnEachInParallel<TItem>(this IActivationDiagnostics diagnostics, IEnumerable<TItem> targets, Action<TItem, IActivationLog> continuation)
        {
            var tasks = targets.Select(currentTarget => {
                return Task.Factory.StartNew(() => {
                    var log = diagnostics.LogFor(currentTarget);
                    diagnostics.LogExecution(currentTarget, () => continuation(currentTarget, log));
                });
            }).ToArray();

            Task.WaitAll(tasks);
        }


    }
}