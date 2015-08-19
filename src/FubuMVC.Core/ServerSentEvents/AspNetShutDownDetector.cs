using System;
using System.Web.Hosting;

namespace FubuMVC.Core.ServerSentEvents
{
    // TODO: Move to more appropriate location.
    public interface IAspNetShutDownDetector : IRegisteredObject, IDisposable
    {
        void Register(Action onShutdown);
    }

    public class AspNetShutDownDetector : IAspNetShutDownDetector
    {
        private Action _onShutdown;

        public void Register(Action onShutdown)
        {
            _onShutdown = onShutdown;
            HostingEnvironment.RegisterObject(this);
        }

        public void Stop(bool immediate)
        {
            try
            {
                if (_onShutdown != null)
                    _onShutdown();
            }
            catch
            {
                // Swallow the exception as Stop should never throw
                // TODO: Log exceptions
            }
            finally
            {
                Dispose();
            }
        }

        public void Dispose()
        {
            HostingEnvironment.UnregisterObject(this);
            _onShutdown = null;
        }
    }
}