using System;
using System.Collections.Generic;
using FubuCore;

namespace FubuMVC.Core.Packaging.Environment
{
    public class EnvironmentRunner
    {
        private readonly EnvironmentRun _run;

        public EnvironmentRunner(EnvironmentRun run)
        {
            _run = run;
        }

        public IEnumerable<LogEntry> ExecuteEnvironment(params Action<IInstaller, IPackageLog>[] actions)
        {
            var list = new List<LogEntry>();

            var environment = findEnvironment(list);
            
            if (environment != null)
            {
                startTheEnvironment(list, environment, actions);    
            }

            return list;
        }

        private static void startTheEnvironment(IList<LogEntry> list, IEnvironment environment, params Action<IInstaller, IPackageLog>[] actions)
        {
            var log = new PackageRegistryLog();
            
            try
            {
                var installers = environment.StartUp(log);

                // This needs to happen regardless, but we want these logs put in before
                // logs for the installers, so we don't do it in the finally{}
                AddPackagingLogEntries(list);

                executeInstallers(list, installers, actions);
                
            }
            catch (Exception ex)
            {
                AddPackagingLogEntries(list);
                log.MarkFailure(ex.ToString());
            }
            finally
            {
                list.Add(LogEntry.FromPackageLog(environment, log));
                environment.SafeDispose();
            }
        }

        private static void executeInstallers(IList<LogEntry> list, IEnumerable<IInstaller> installers, IEnumerable<Action<IInstaller, IPackageLog>> actions)
        {
            foreach (var action in actions)
            {
                foreach (var installer in installers)
                {
                    var log = new PackageRegistryLog();
                    try
                    {
                        action(installer, log);
                    }
                    catch (Exception e)
                    {
                        log.MarkFailure(e.ToString());
                    }

                    list.Add(installer, log);
                }
            }
        }

        private IEnvironment findEnvironment(List<LogEntry> list)
        {
            var environmentType = _run.FindEnvironmentType();
            if (environmentType == null)
            {
                throw new EnvironmentRunnerException("Unable to find an IEnvironment type");
            }

            IEnvironment environment = null;
            try
            {
                environment = (IEnvironment) Activator.CreateInstance(environmentType);

            }
            catch (Exception e)
            {
                list.Add(new LogEntry
                         {
                             Description = environmentType.FullName,
                             Success = false,
                             TraceText = e.ToString()
                         });
            }

            return environment;
        }

        public static void AddPackagingLogEntries(IList<LogEntry> list)
        {
            if (PackageRegistry.Diagnostics != null)
            {
                PackageRegistry.Diagnostics.EachLog((target, log) => list.Add(target, log));
            }
        }
    }
}