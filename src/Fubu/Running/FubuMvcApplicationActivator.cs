using System;
using FubuCore;
using FubuMVC.Core.Http.Hosting;
using FubuMVC.Core.Runtime;

namespace Fubu.Running
{
    public class FubuMvcApplicationActivator : IFubuMvcApplicationActivator
    {
        private int _port;
        private string _physicalPath;
        private readonly EmbeddedFubuMvcServer _server = null;

        public void Initialize(Type applicationType, int port, string physicalPath)
        {
            //_applicationSource = Activator.CreateInstance(applicationType).As<FubuRegistry>();
            _port = PortFinder.FindPort(port);
            _physicalPath = physicalPath;

            StartUp();
        }

        public void StartUp()
        {
            throw new Exception("NWO");

            /*

            try
            {
                
                var application = _applicationSource.BuildApplication(_physicalPath);
                var runtime = application;
                _server = new EmbeddedFubuMvcServer(runtime, new KatanaHost(), _port);

                EventAggregator.SendMessage(new ApplicationStarted
                {
                    ApplicationName = _applicationSource.GetType().Name,
                    HomeAddress = _server.BaseAddress,
                    Timestamp = DateTime.Now,
                    Watcher = runtime.Factory.Get<AssetSettings>().CreateFileWatcherManifest(runtime.Files)
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
             * */
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