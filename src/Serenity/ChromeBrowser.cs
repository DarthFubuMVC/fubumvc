using System;
using FubuCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Serenity
{
    public class ChromeBrowser : BrowserLifecycle
    {
        public const string ChromeProcess = "chrome";
        public const string DriverProcess = "chromedriver";
        public const string File = "chromedriver.exe";

        public override string BrowserName => "Chrome";

        protected override IWebDriver buildDriver()
        {
            var fileSystem = new FileSystem();

            var workingDir = AppDomain.CurrentDomain.BaseDirectory;
            return fileSystem.FileExists(workingDir, File)
                ? new ChromeDriver(workingDir)
                : new ChromeDriver();
        }

        protected override void aggressiveCleanup()
        {
            Kill.Processes(DriverProcess, File, ChromeProcess);
        }
    }
}