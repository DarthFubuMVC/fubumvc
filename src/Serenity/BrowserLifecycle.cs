using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using FubuCore;
using OpenQA.Selenium;

namespace Serenity
{
    public abstract class BrowserLifecycle : IBrowserLifecycle
    {
        private Lazy<IWebDriver> _driver;
        private readonly IList<IBrowserSessionInitializer> _initializers = new List<IBrowserSessionInitializer>();

        // This is a stop gap to deal with instantiating multiple instances of any BrowserLifecycle within one process concurrently.
        // Ideally we should create the different WebDriverService instances in singleton scope, then create WebDrivers pointing at 
        // the existing service. The Serenity.Test project is an example consumer of this need. 
        // Chrome See: https://sites.google.com/a/chromium.org/chromedriver/getting-started for setup with Chrome specifically 
        //             the Controlling ChromeDriver's lifetime section - Note the ChromeDriverService usage
        // Firefox need to investigate best approach for this.
        private static readonly object _runningLifecyclesLock = new object();
        private static readonly IDictionary<Type, int> _runningLifecycles = new Dictionary<Type, int>();

        public abstract string BrowserName { get; }

        protected BrowserLifecycle()
        {
            Recycle();
        }

        private bool cleanUpFlag()
        {
            bool cleanUp;
            Boolean.TryParse(Environment.GetEnvironmentVariable("serenityci"), out cleanUp);
            return cleanUp;
        }

        private IWebDriver initializeDriver()
        {
            if (cleanUpFlag()) { aggressiveCleanup(); }

            var driver = BuildDriverAndIncrementLifecycleCount();
            _initializers.Each(x => x.InitializeSession(driver));

            return driver;
        }

        protected abstract IWebDriver buildDriver();

        public void Dispose()
        {
            if (_driver == null || !_driver.IsValueCreated)
                return;

            lock (_runningLifecyclesLock)
            {
                var type = GetType();
                if (!_runningLifecycles.ContainsKey(type))
                {
                    throw new Exception("Should never happen, but if you try to decrement the lifecycle and the key does not exist");
                }

                _runningLifecycles[type]--;

                var task = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        _driver.Value.Quit();
                        _driver.Value.Dispose();
                        _driver = null;
                        return true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        return false;
                    }
                });

                var timeout = TimeSpan.FromMinutes(1);

                var timedout = !task.Wait(timeout);
                var failed = false;

                if (!timedout)
                {
                    failed = !task.Result;
                }

                if (_runningLifecycles[type] < 0)
                {
                    throw new Exception("Decrement has been called too many times?");
                }

                if (_runningLifecycles[type] == 0 && (timedout || failed))
                {
                    Console.WriteLine("{0} Cleanup {1} proceeding with aggressive cleanup (Killing Processes)",
                        BrowserName,
                        timedout
                            ? "timed out after {0:c}".ToFormat(timeout)
                            : "failed");
                    aggressiveCleanup();
                    _driver = null;
                }
            }
        }

        ~BrowserLifecycle()
        {
            this.SafeDispose();
        }

        protected abstract void aggressiveCleanup();

        public void UseInitializer(IBrowserSessionInitializer initializer)
        {
            _initializers.Add(initializer);
        }

        public IWebDriver Driver
        {
            get { return _driver.Value; }
        }

        public void Recycle()
        {
            Dispose();
            _driver = new Lazy<IWebDriver>(initializeDriver, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public bool HasBeenStarted()
        {
            return _driver.IsValueCreated;
        }


        private IWebDriver BuildDriverAndIncrementLifecycleCount()
        {
            lock (_runningLifecyclesLock)
            {
                var type = GetType();
                if (!_runningLifecycles.ContainsKey(type))
                {
                    _runningLifecycles[type] = 0;
                }

                _runningLifecycles[type]++;

                return buildDriver();
            }
        }
    }
}