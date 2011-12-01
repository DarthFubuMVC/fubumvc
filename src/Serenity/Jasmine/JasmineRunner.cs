using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Bottles;
using FubuCore;
using FubuKayak;
using FubuMVC.Core;
using FubuMVC.Core.Assets.Caching;
using OpenQA.Selenium;

namespace Serenity.Jasmine
{
    public class JasmineRunner : ISpecFileListener
    {
        private readonly JasmineInput _input;
        private readonly ManualResetEvent _reset = new ManualResetEvent(false);
        private SerenityJasmineApplication _application;
        private ApplicationUnderTest _applicationUnderTest;
        private ApplicationDriver _driver;
        private FubuKayakApplication _kayak;
        private Thread _kayakLoop;
        private AssetFileWatcher _watcher;


        public JasmineRunner(JasmineInput input)
        {
            _input = input;
        }

        void ISpecFileListener.Changed()
        {
            _applicationUnderTest.Driver.Navigate().Refresh();
        }

        void ISpecFileListener.Deleted()
        {
            _kayak.Recycle(watchAssetFiles);
            // TODO -- make a helper method for this
            _applicationUnderTest.Driver.Navigate().GoToUrl(_applicationUnderTest.RootUrl);
        }

        void ISpecFileListener.Added()
        {
            Recycle();
        }

        public void Recycle()
        {
            _kayak.Recycle(watchAssetFiles);
            _applicationUnderTest.Driver.Navigate().Refresh();
        }

        public void OpenInteractive()
        {
            buildApplication();

            var threadStart = new ThreadStart(run);
            _kayakLoop = new Thread(threadStart);
            _kayakLoop.Start();


            // TODO -- make a helper method for this
            _driver.NavigateToHome();

            _reset.WaitOne();
        }

        public bool RunAllSpecs()
        {
            var title = "Running Jasmine specs for project at " + _input.SerenityFile;
            Console.WriteLine(title);
            var line = "".PadRight(title.Length, '-');

            Console.WriteLine(line);

            buildApplication();
            var returnValue = true;

            _kayak = new FubuKayakApplication(_application);
            _kayak.RunApplication(_input.PortFlag, runtime =>
            {
                _driver.NavigateTo<JasminePages>(x => x.AllSpecs());

                var browser = _applicationUnderTest.Driver;
                var failures = browser.FindElements(By.CssSelector("div.suite.failed"));


                if (failures.Any())
                {
                    returnValue = false;

                    Console.WriteLine(line);
                    writeFailures(failures);
                }

                Console.WriteLine();
                Console.WriteLine(line);
                writeTotals(browser);

                _kayak.Stop();

                browser.Quit();
            });


            return returnValue;
        }

        private static void writeTotals(IWebDriver browser)
        {
            var totals = browser.FindElement(By.CssSelector("div.jasmine_reporter a.description")).Text;

            Console.WriteLine(totals);
        }


        private static void writeFailures(IEnumerable<IWebElement> failures)
        {
            failures.Each(suite =>
            {
                Console.WriteLine(suite.FindElement(By.CssSelector("a.description")).Text);

                suite.FindElements(By.CssSelector("div.spec.failed a.description"))
                    .Each(spec => Console.WriteLine("    " + spec.Text));
            });
        }

        private void run()
        {
            _kayak = new FubuKayakApplication(_application);
            _kayak.RunApplication(_input.PortFlag, watchAssetFiles);

            _reset.Set();
        }

        private void watchAssetFiles(FubuRuntime runtime)
        {
            if (_watcher == null)
            {
                _watcher = runtime.Facility.Get<AssetFileWatcher>();
                _watcher.StartWatching(this);
            }
        }


        private void buildApplication()
        {
            _application = new SerenityJasmineApplication();
            var configLoader = new ConfigFileLoader(_input.SerenityFile, _application);
            configLoader.ReadFile();


            var applicationSettings = new ApplicationSettings{
                RootUrl = "http://localhost:" + _input.PortFlag
            };

            var browserBuilder = _input.GetBrowserBuilder();

            _applicationUnderTest = new ApplicationUnderTest(_application, applicationSettings, browserBuilder);

            _driver = new ApplicationDriver(_applicationUnderTest);
        }
    }

    public class AssetFileWatcher
    {
        private readonly IAssetContentCache _cache;
        private readonly IFileSystem _fileSystem = new FileSystem();
        private readonly IList<FileSystemWatcher> _watchers = new List<FileSystemWatcher>();

        public AssetFileWatcher(IAssetContentCache cache)
        {
            _cache = cache;
        }

        public void StartWatching(ISpecFileListener listener)
        {
            PackageRegistry.Packages.Each(pak =>
            {
                pak.ForFolder(BottleFiles.WebContentFolder, dir =>
                {
                    var contentFolder = dir.AppendPath("content");
                    if (_fileSystem.DirectoryExists(contentFolder))
                    {
                        addContentFolder(contentFolder, listener);
                    }

                    var watcher = new FileSystemWatcher(dir, "*.config");
                    watcher.Changed += (x, y) => listener.Recycle();
                    watcher.Deleted += (x, y) => listener.Recycle();
                    watcher.EnableRaisingEvents = true;
                    watcher.IncludeSubdirectories = true;

                    _watchers.Add(watcher);
                });
            });
        }

        private void addContentFolder(string dir, ISpecFileListener listener)
        {
            var watcher = new FileSystemWatcher(dir);
            watcher.Changed += (x, file) =>
            {
                Console.WriteLine("Detected a change to " + file.FullPath);

                _cache.FlushAll();
                listener.Changed();
            };

            watcher.Created += (x, y) =>
            {
                Console.WriteLine("Detected a new file at " + y.FullPath);
                listener.Added();
            };
            watcher.Deleted += (x, y) =>
            {
                Console.WriteLine("Detected a file deletion at " + y.FullPath);
                listener.Deleted();
            };

            watcher.EnableRaisingEvents = true;
            watcher.IncludeSubdirectories = true;
        }

        public void StopWatching()
        {
            _watchers.Each(x => x.SafeDispose());
        }
    }

    public interface ISpecFileListener
    {
        void Changed();
        void Deleted();
        void Added();
        void Recycle();
    }
}