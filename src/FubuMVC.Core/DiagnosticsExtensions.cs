using System;
using System.IO;
using FubuCore;
using FubuMVC.Core.Diagnostics.Packaging;

namespace FubuMVC.Core
{
    public static class DiagnosticsExtensions
    {
        /// <summary>
        /// A static method that should be exposed, to allow you to 
        /// take an action when there has been a failure in the system.
        /// </summary>
        /// <param name="failure">The action to perform</param>
        public static void AssertNoFailures(this IActivationDiagnostics diagnostics, Action failure)
        {
            if (diagnostics.HasErrors())
            {
                failure();
            }
        }

        /// <summary>
        /// Default AssertNoFailures
        /// </summary>
        public static void AssertNoFailures(this IActivationDiagnostics diagnostics)
        {
            diagnostics.AssertNoFailures(() =>
            {
                var writer = new StringWriter();
                writer.WriteLine("Package loading and application bootstrapping failed");
                writer.WriteLine();
                diagnostics.EachLog((o, log) =>
                {
                    if (!log.Success)
                    {
                        writer.WriteLine(o.ToString());
                        writer.WriteLine(log.FullTraceText());
                        writer.WriteLine(
                            "------------------------------------------------------------------------------------------------");
                    }
                });

                throw new FubuException(1, writer.GetStringBuilder().ToString());
            });
        }
    }
}