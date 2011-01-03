using System;
using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.Packaging.Environment;

namespace Fubu
{
    public class InstallationLogger : IInstallationLogger
    {
        public void WriteLogsToConsole(IEnumerable<LogEntry> entries)
        {
            entries.Each(writeLog);
        }

        public void WriteLogsToFile(InstallInput input, IEnumerable<LogEntry> entries)
        {
            var document = EntryLogWriter.Write(entries, input.Title() + " at " + DateTime.UtcNow.ToLongDateString());
            document.WriteToFile(input.LogFileFlag);

            Console.WriteLine("Output writing to {0}", input.LogFileFlag.ToFullPath());

            if (input.OpenFlag)
            {
                document.OpenInBrowser();
            }
        }

        public void WriteSuccessToConsole()
        {
            Console.WriteLine("All installers succeeded without problems");
        }

        public void WriteFailureToConsole()
        {
            throw new CommandFailureException("Failures occurred.  Please see the log file");
        }

        private void writeLog(LogEntry log)
        {
            Console.WriteLine("{0}, Success = {1}", log.Description, log.Success);
            Console.WriteLine(log.TraceText);

            Console.WriteLine();
            Console.WriteLine();
        }
    }
}