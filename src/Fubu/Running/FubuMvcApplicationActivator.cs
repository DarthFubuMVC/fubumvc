using System;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Http.Hosting;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Http.Owin.Middleware;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Services.Messaging;

namespace Fubu.Running
{
    public class FubuMvcApplicationActivator : IFubuMvcApplicationActivator
    {
        private FubuRuntime _server = null;
        private FubuRegistry _registry;

        public void Initialize(Type applicationType, StartApplication message)
        {
            _registry = Activator.CreateInstance(applicationType).As<FubuRegistry>();
            _registry.RootPath = message.PhysicalPath;
            _registry.Port = PortFinder.FindPort(message.PortNumber);
            _registry.Mode = message.Mode;

            _registry.AlterSettings<OwinSettings>(owin =>
            {
                owin.AddMiddleware<HtmlHeadInjectionMiddleware>().Arguments.With(new InjectionOptions
                {
                    Content = c => message.HtmlHeadInjectedText
                });
            });

            if (_registry.Host == null) _registry.HostWith<Katana>();

            StartUp();
        }

        public void StartUp()
        {
            try
            {
                _server = _registry.ToRuntime();

                EventAggregator.SendMessage(new ApplicationStarted
                {
                    ApplicationName = _registry.GetType().Name,
                    HomeAddress = _server.BaseAddress,
                    Timestamp = DateTime.Now,
                    Watcher = _server.Get<AssetSettings>().CreateFileWatcherManifest(_server.Files)
                });
            }
            catch (AdminRightsException e)
            {
                EventAggregator.SendMessage(new InvalidApplication
                {
                    ExceptionText = e.Message,
                    Message = "Access denied."
                });               
            }
            catch (Exception e)
            {
                EventAggregator.SendMessage(new InvalidApplication
                {
                    ExceptionText = e.ToString(),
                    Message = "Bootstrapping {0} Failed!".ToFormat(_registry.GetType().Name)
                });
            }
        }

        public void ShutDown()
        {
            _server.SafeDispose();
        }

        public void Recycle()
        {
            ShutDown();
            StartUp();
        }
    }
}