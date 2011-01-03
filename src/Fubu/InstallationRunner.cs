using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Packaging.Environment;

namespace Fubu
{
    public class InstallationRunner
    {
        private readonly IEnvironmentGateway _gateway;
        private readonly IInstallationLogger _logger;

        public InstallationRunner(IEnvironmentGateway gateway, IInstallationLogger logger)
        {
            _gateway = gateway;
            _logger = logger;
        }

        // TODO -- this needs to fail!
        public void RunTheInstallation(InstallInput input)
        {
            var entries = execute(input);
            logEntries(input, entries);
        }

        private void logEntries(InstallInput input, IEnumerable<LogEntry> entries)
        {
            _logger.WriteLogsToConsole(entries);
            _logger.WriteLogsToFile(input, entries);

            if (entries.Any(x => !x.Success))
            {
                _logger.WriteFailureToConsole();
            }
            else
            {
                _logger.WriteSuccessToConsole();
            }
        }

        private IEnumerable<LogEntry> execute(InstallInput input)
        {
            Console.WriteLine(input.Title());

            switch (input.ModeFlag)
            {
                case InstallMode.install:
                    return _gateway.Install();

                case InstallMode.check:
                    return _gateway.CheckEnvironment();

                case InstallMode.all:
                    return _gateway.InstallAndCheckEnvironment();
            }

            return new LogEntry[0];
        }
    }
}