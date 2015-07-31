using System;
using System.Collections.Generic;
using FubuMVC.Core;
using FubuCore;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuMVC.Core.Http.Owin.Middleware;
using FubuMVC.Core.Services;
using FubuMVC.Core.Services.Messaging;

namespace Fubu.Running
{
    public class RemoteFubuMvcBootstrapper : IApplicationLoader, IActivator, IListener<StartApplication>, IListener<RecycleApplication>, IDisposable
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

        public IDisposable Load()
        {
            EventAggregator.Messaging.AddListener(this);
            return this;
        }

        public void Dispose()
        {
            _activator.ShutDown();
        }

        public void Receive(StartApplication message)
        {
            Console.WriteLine("Trying to start application " + message);

            FubuRuntime.Properties[HtmlHeadInjectionMiddleware.TEXT_PROPERTY] = message.HtmlHeadInjectedText;

            if (message.Mode.IsNotEmpty())
            {
                FubuMode.Mode(message.Mode);
            }

            Console.WriteLine("FubuMode = " + FubuMode.Mode());

            var chooser = new ApplicationSourceChooser(_typeFinder, _messaging);
            chooser.Find(message, applicationType => {
                _activator.Initialize(applicationType, message.PortNumber, message.PhysicalPath);
            });
        }

        public void Receive(RecycleApplication message)
        {
            _activator.Recycle();
        }

        void IActivator.Activate(IActivationLog log, IPerfTimer timer)
        {
        }
    }
}