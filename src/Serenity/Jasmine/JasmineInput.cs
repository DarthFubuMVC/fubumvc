using System;
using System.Collections.Generic;
using System.ComponentModel;
using FubuCore.CommandLine;
using OpenQA.Selenium;


namespace Serenity.Jasmine
{
    public enum JasmineMode
    {
        interactive,
        run,
        add_folders
        
    }

    public class JasmineInput
    {
        public JasmineInput()
        {
            PortFlag = 5500;
            BrowserFlag = BrowserType.Chrome;

        }

        [Description("Chooses whether to open the browser application or just run all the specs in CLI mode")]
        [RequiredUsage("default", "add_folders")]
        public JasmineMode Mode { get; set; }

        [Description("Name of the file containing directives for where the specifications are located")]
        [RequiredUsage("default", "add_folders")]
        public string SerenityFile { get; set; }

        [Description("Optionally overrides which port number Kayak uses for the web application.  Default is 5500")]
        public int PortFlag { get; set; }

        [Description("Choose which browser to use for the testing")]
        public BrowserType BrowserFlag { get; set; }

        [Description("Adds folders to a Jasmine project in the add_folders.  \nFolders can be either absolute paths or relative to the jasmine text file")]
        [RequiredUsage("add_folders")]
        public IEnumerable<string> Folders { get; set; }

        public Func<IWebDriver> GetBrowserBuilder()
        {
            return WebDriverSettings.DriverBuilder(BrowserFlag);
        }
    }
}