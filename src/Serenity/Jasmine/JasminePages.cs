using System;
using System.Diagnostics;
using FubuCore.CommandLine;
using FubuMVC.Core;
using FubuMVC.OwinHost;
using FubuMVC.StructureMap;
using HtmlTags;
using OpenQA.Selenium.Chrome;
using StructureMap;

namespace Serenity.Jasmine
{
    public class InteractiveJasmineInput
    {
        public InteractiveJasmineInput()
        {
            PortFlag = 5500;
        }

        public int PortFlag { get; set; }
    }

    [CommandDescription("Opens up a web browser application to browse and execute Jasmine specifications", Name = "interactive-jasmine")]
    public class InteractiveJasmineCommand : FubuCommand<InteractiveJasmineInput>
    {
        public override bool Execute(InteractiveJasmineInput input)
        {
            var host = new FubuOwinHost(new SerenityJasmineApplication());

            Console.WriteLine("Opening the Serenity Jasmine Runner on port " + input.PortFlag);


            var driver = new ChromeDriver();
            driver.Navigate().GoToUrl("http://cnn.com");


            Debug.WriteLine("got a window");

            host.RunApplication(input.PortFlag);

            return true;
        }
    }

    public class JasminePages
    {
        public HtmlDocument Home()
        {
            var document = new HtmlDocument{
                Title = "Serenity Jasmine Tester"
            };

            document.Add("h1").Text("Serenity Jasmine Tester");


            return document;
        }
    }

    public class SerenityJasmineRegistry : FubuRegistry
    {
        public SerenityJasmineRegistry()
        {
            Actions.IncludeType<JasminePages>();
            Routes.HomeIs<JasminePages>(x => x.Home());
        }
    }

    public class SerenityJasmineApplication : IApplicationSource
    {
        public FubuApplication BuildApplication()
        {
            // TODO -- need to add more stuff here
            return FubuApplication
                .For<SerenityJasmineRegistry>()
                .StructureMap(new Container());
        }

        public string Name
        {
            get { return "Serenity Jasmine Runner"; }
        }
    }

    public class SerenityAppHost
    {
        public void AddDirectory(string folder)
        {
            throw new NotImplementedException();
        }
    }
}