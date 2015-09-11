using System;
using System.Collections.Generic;
using FubuMVC.Core.Services.Messaging;

namespace FubuMVC.Core.Services.Remote
{
    public class LoaderStarted
    {
        public string LoaderTypeName { get; set; }
    }

    public class RemoteServicesProxy : MarshalByRefObject
    {
        private IDisposable _shutdown;

        public void Start(string bootstrapperName, MarshalByRefObject remoteListener)
        {
            Start(bootstrapperName, new Dictionary<string, string>(), remoteListener);
        }

        public void Start(string bootstrapperName, Dictionary<string, string> properties, MarshalByRefObject remoteListener)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            //var domainSetup = AppDomain.CurrentDomain.SetupInformation;
            
            // TODO -- what the hell here? This is bad bad bad.
            //System.Environment.CurrentDirectory = domainSetup.ApplicationBase;
             
            // TODO -- need to handle exceptions gracefully here
            EventAggregator.Start((IRemoteListener) remoteListener);

            var loader = ApplicationLoaderFinder.FindLoader(bootstrapperName);
            _shutdown = loader.Load(properties);

            EventAggregator.SendMessage(new LoaderStarted
            {
                LoaderTypeName = _shutdown.GetType().FullName
            });
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine("An an unhandled exception occurred at the remote AppDomain hosted at " + AppDomain.CurrentDomain.BaseDirectory);
            Console.WriteLine(e.ExceptionObject.ToString());
        }

        public void Shutdown()
        {
            EventAggregator.Stop();
            if (_shutdown != null) _shutdown.Dispose();
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void SendJson(string json)
        {
            EventAggregator.Messaging.SendJson(json);
        }
    }
}