using System;
using System.Diagnostics;
using FubuCore;
using FubuCore.Util;

namespace Bottles.Deployment.Diagnostics
{
    public class DeploymentDiagnostics : IDeploymentDiagnostics
    {
        private readonly Cache<object, PackageDeploymentLog> _logs = new Cache<object, PackageDeploymentLog>(o => new PackageDeploymentLog()
        {
            Description = o.ToString()
        });

        public void LogObject(object target, string provenance)
        {
            _logs[target].Provenance = provenance;
        }
        public PackageDeploymentLog LogFor(object target)
        {
            return _logs[target];
        }

        public void LogInitialization(IInitializer initializer, IDirective directive)
        {
            LogObject(initializer, "Running initializer for directive '{0}'".ToFormat(directive));
            LogFor(directive).AddChild(initializer);
        }

        public void LogFinalization(IFinalizer finalizer, IDirective directive)
        {
            LogObject(finalizer, "Running finalizer for directive '{0}'".ToFormat(directive));
            LogFor(directive).AddChild(finalizer);
        }

        public void LogDeployment(IDeployer deployer, IDirective directive)
        {
            LogObject(deployer, "Running with directive '{0}'".ToFormat(directive));
            LogFor(directive).AddChild(deployer);
        }

        public void LogHost(HostManifest hostManifest)
        {
            LogObject(hostManifest, "Deploying host from deployment ???");
            LogFor("deploymentname").AddChild(hostManifest);
        }

        public void LogDirective(IDirective directive, HostManifest host)
        {
            LogObject(directive, "Found in '{0}'".ToFormat(host));
            LogFor(host).AddChild(directive);
        }

        public void LogDeployer(IDeployer deployer, HostManifest host,  Action<IDeployer> action)
        {
            LogObject(deployer, "Running for host '{0}'".ToFormat(host.Name));
            LogFor(host).AddChild(deployer);
        }

        public void LogFinalizer(IFinalizer finalizer, HostManifest host, Action<IFinalizer> action)
        {
            LogObject(finalizer, "Running for host '{0}'".ToFormat(host.Name));
            LogFor(host).AddChild(finalizer);
        }

        public void LogInitializer(IInitializer initializer, HostManifest host, Action<IInitializer> action)
        {
            LogObject(initializer, "Running for host '{0}'".ToFormat(host.Name));
            LogFor(host).AddChild(initializer);
        }

        public void LogExecution(object target, string description, Action continuation)
        {
            var log = _logs[target];
            log.Description = description;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                continuation();
            }
            catch (Exception e)
            {
                log.MarkFailure(e);
            }
            finally
            {
                stopwatch.Stop();
                log.TimeInMilliseconds = stopwatch.ElapsedMilliseconds;
            }
        }
    }
}