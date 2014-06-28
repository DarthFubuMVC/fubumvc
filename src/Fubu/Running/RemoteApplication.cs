using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Bottles.Services.Messaging;
using Bottles.Services.Remote;
using FubuCore;
using FubuCore.CommandLine;
using FubuMVC.Core;
using FubuMVC.Core.Runtime;

namespace Fubu.Running
{
    public class RemoteApplication : IListener<ApplicationStarted>, IListener<InvalidApplication>, IApplicationObserver
    {
        private readonly ManualResetEvent _reset = new ManualResetEvent(false);
        private readonly BrowserDriver _driver = new BrowserDriver();
        private ApplicationRequest _input;
        private bool _opened;
        private RemoteFubuMvcProxy _proxy;
        private FileWatcherManifest _watcher;
        private readonly Action<RemoteDomainExpression> _configuration;


        public RemoteApplication()
        {
            _configuration = x => { };
        }

        public RemoteApplication(Action<RemoteDomainExpression> configuration)
        {
            _configuration = configuration;
        }

        public void RefreshContent()
        {
            _driver.RefreshPage();
        }

        public void RecycleAppDomain()
        {
            if (_watcher != null) _watcher.StopWatching();
            if (_proxy != null)
            {
                _proxy.SafeDispose();
            }

            start();

            _driver.RefreshPage();
        }

        public void RecycleApplication()
        {
            _watcher.StopWatching();
            _proxy.Recycle();

            _driver.RefreshPage();
        }

        public void Receive(ApplicationStarted message)
        {
            Console.WriteLine("Started application {0} at url {1} at {2}", message.ApplicationName, message.HomeAddress,
                message.Timestamp);


            if ((_input.OpenFlag || _input.UrlFlag.IsNotEmpty()) && !_opened)
            {
                _opened = true;
                var url = message.HomeAddress;
                if (_input.UrlFlag.IsNotEmpty())
                {
                    url = url.TrimEnd('/') + '/' + _input.UrlFlag.TrimStart('/');
                }

                _driver.OpenBrowserTo(url);
            }

            if (_watcher == null)
            {
                _watcher = message.Watcher;
                _watcher.Watch(_input.WatchedFlag, this);
            }

            _watcher.StartWatching();


            _reset.Set();
        }

        public void Receive(InvalidApplication message)
        {
            ConsoleWriter.Write(ConsoleColor.Red, message.Message);

            if (message.Applications != null && message.Applications.Any())
            {
                Console.WriteLine("Found applications:  " + message.Applications.Join(", "));
            }

            if (message.ExceptionText.IsNotEmpty())
            {
                ConsoleWriter.Write(ConsoleColor.Yellow, message.ExceptionText);
            }

            _reset.Set();

            Failed = true;

            throw new Exception("Application Failed!");
        }

        public bool Failed { get; set; }

        public void Start(ApplicationRequest input)
        {
            _input = input;

            if (_input.WatchedFlag)
            {
                _driver.StartWebSockets();
                _input.AutoRefreshWebSocketsAddress = _driver.Port.ToString();
            }

            start();
        }

        private void start()
        {
            _reset.Reset();
            _proxy = new RemoteFubuMvcProxy(_input);
            _proxy.Start(this, _configuration);

            _reset.WaitOne();
        }

        public void Shutdown()
        {
            _driver.SafeDispose();
            _watcher.StopWatching();
            _proxy.SafeDispose();
        }

        public void GenerateTemplates()
        {
            _proxy.GenerateTemplates();
        }
    }


}