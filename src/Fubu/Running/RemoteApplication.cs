using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Bottles.Services.Messaging;
using Bottles.Services.Remote;
using FubuCore;
using FubuCore.CommandLine;
using FubuMVC.Core;

namespace Fubu.Running
{
    public class RemoteApplication : IListener<ApplicationStarted>, IListener<InvalidApplication>, IApplicationObserver
    {
        private readonly ManualResetEvent _reset = new ManualResetEvent(false);
        private readonly BrowserDriver _driver = new BrowserDriver();
        private ApplicationRequest _input;
        private bool _opened;
        private RemoteFubuMvcProxy _proxy;
        private FubuMvcApplicationFileWatcher _watcher;
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
            _watcher.StopWatching();
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



            if (_input.OpenFlag && !_opened)
            {
                _opened = true;
                _driver.OpenBrowserTo(message.HomeAddress);
            }

            _watcher.StartWatching(_input.DirectoryFlag, message.BottleContentFolders);

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

            // TODO -- need to add the FileWatcherManifest before starting to do anything here.
            _watcher = new FubuMvcApplicationFileWatcher(this, new FileMatcher(new FileWatcherManifest()));

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