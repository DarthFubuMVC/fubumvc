using System;
using System.Diagnostics;
using FubuCore.CommandLine;
using FubuMVC.OwinHost;
using OpenQA.Selenium.Chrome;

namespace Serenity.Jasmine
{
    [CommandDescription("Opens up a web browser application to browse and execute Jasmine specifications", Name = "interactive-jasmine")]
    public class InteractiveJasmineCommand : FubuCommand<InteractiveJasmineInput>
    {
        public override bool Execute(InteractiveJasmineInput input)
        {
            // TODO -- tighten up the defensive programming against bad input
            var runner = new JasmineRunner(input);
            runner.Run();

            return true;
        }
    }
}