using System;
using System.Collections.Generic;
using Bottles;
using Bottles.Services.Messaging;
using FubuCore;
using FubuCsProjFile.Templating.Graph;
using FubuMVC.Core;
using FubuMVC.Core.Assets;
using FubuMVC.Katana;

namespace Fubu.Running
{
    public class FubuMvcApplicationActivator : IFubuMvcApplicationActivator
    {
        private IApplicationSource _applicationSource;
        private int _port;
        private string _physicalPath;
        private EmbeddedFubuMvcServer _server;

        public void Initialize(Type applicationType, int port, string physicalPath)
        {
            _applicationSource = Activator.CreateInstance(applicationType).As<IApplicationSource>();
            _port = PortFinder.FindPort(port);
            _physicalPath = physicalPath;

            StartUp();
        }

        public void StartUp()
        {
            try
            {
                var application = _applicationSource.BuildApplication();
                var runtime = application.Bootstrap();
                _server = new EmbeddedFubuMvcServer(runtime, _physicalPath, _port);

                EventAggregator.SendMessage(new ApplicationStarted
                {
                    ApplicationName = _applicationSource.GetType().Name,
                    HomeAddress = _server.BaseAddress,
                    Timestamp = DateTime.Now,
                    Watcher = runtime.Factory.Get<AssetSettings>().CreateFileWatcherManifest()
                });
            }
            catch (KatanaRightsException e)
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
                    Message = "Bootstrapping {0} Failed!".ToFormat(_applicationSource.GetType().Name)
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

        public void GenerateTemplates()
        {
            _server.Services.Get<FubuMVC.Core.Assets.Templates.TemplateGraph>().WriteAll();
        }
    }
}