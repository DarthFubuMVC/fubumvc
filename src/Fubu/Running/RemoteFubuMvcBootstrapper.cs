using System;
using System.Collections.Generic;
using Bottles;
using Bottles.Diagnostics;
using Bottles.Services.Messaging;
using FubuMVC.Core;
using FubuCore;
using FubuMVC.Core.Http.Owin.Middleware;

namespace Fubu.Running
{
    public class RemoteFubuMvcBootstrapper : IBootstrapper, IActivator, IDeactivator, IListener<StartApplication>, IListener<RecycleApplication>, IListener<GenerateTemplates>
    {
        private readonly IApplicationSourceFinder _typeFinder;
        private readonly IFubuMvcApplicationActivator _activator;
        private readonly IMessaging _messaging;

        public RemoteFubuMvcBootstrapper() : this(new ApplicationSourceFinder(), new FubuMvcApplicationActivator(), new Messaging())
        {
        }

        public RemoteFubuMvcBootstrapper(IApplicationSourceFinder typeFinder, IFubuMvcApplicationActivator activator, IMessaging messaging)
        {
            _typeFinder = typeFinder;
            _activator = activator;
            _messaging = messaging;
        }

        public IEnumerable<IActivator> Bootstrap(IPackageLog log)
        {
            yield return this;
        }

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            EventAggregator.Messaging.AddListener(this);
        }

        public void Deactivate(IPackageLog log)
        {
            _activator.ShutDown();
        }


        public void Receive(StartApplication message)
        {
            Console.WriteLine("Trying to start application " + message);

            PackageRegistry.Properties[HtmlHeadInjectionMiddleware.TEXT_PROPERTY] = message.HtmlHeadInjectedText;

            if (message.UseProductionMode)
            {
                Console.WriteLine("FubuMode = Production");
                FubuMode.Reset();
            }
            else
            {
                Console.WriteLine("FubuMode = Development");
                FubuMode.Mode(FubuMode.Development);
            }

            var chooser = new ApplicationSourceChooser(_typeFinder, _messaging);
            chooser.Find(message, applicationType => {
                _activator.Initialize(applicationType, message.PortNumber, message.PhysicalPath);
            });
        }

        public void Receive(RecycleApplication message)
        {
            _activator.Recycle();
        }

        public void Receive(GenerateTemplates message)
        {
            _activator.GenerateTemplates();
        }
    }
}