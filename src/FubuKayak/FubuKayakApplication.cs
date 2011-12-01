using System;
using System.Web.Routing;
using FubuCore;
using FubuMVC.Core;

namespace FubuKayak
{
    public class FubuKayakApplication
    {
        private readonly IApplicationSource _source;
        private Listener _listener;
        private int _port;


        public FubuKayakApplication(IApplicationSource source)
        {
            _source = source;
        }

        public void RunApplication(int port, Action<FubuRuntime> activation)
        {
            _port = port;

            if (_listener != null)
            {
                throw new InvalidOperationException("This FubuKayakApplication is already running");
            }

            startListener(activation);
        }

        private void startListener(Action<FubuRuntime> activation)
        {
            _listener = new Listener(_port);
            var runtime = rebuildFubuMVCApplication();

            _listener.Start(runtime, () => activation(runtime));
        }

        private FubuRuntime rebuildFubuMVCApplication()
        {
            RouteTable.Routes.Clear();
            return _source.BuildApplication().Bootstrap();
        }


        public void Recycle(Action<FubuRuntime> activation)
        {
            shutDownListener();
            
            var runtime = rebuildFubuMVCApplication();

            activation(runtime);
        }

        private void shutDownListener()
        {
            if (_listener != null)
            {
                _listener.SafeDispose();
            }
        }

        public void Stop()
        {
            shutDownListener();
        }


    }

}