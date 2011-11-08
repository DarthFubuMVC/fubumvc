using System;
using FubuCore.CommandLine;

namespace Serenity.Jasmine
{
    [CommandDescription("Opens up a web browser application to browse and execute Jasmine specifications",
        Name = "jasmine")]
    public class JasmineCommand : FubuCommand<JasmineInput>
    {
        public override bool Execute(JasmineInput input)
        {
            // TODO -- tighten up the defensive programming against bad input
            var runner = new JasmineRunner(input);

            if (input.Mode == JasmineMode.interactive)
            {
                runner.OpenInteractive();
            }

            if (input.Mode == JasmineMode.run)
            {
                if (!runner.RunAllSpecs())
                {
                    Console.WriteLine("any key will do");
                    Console.ReadLine();
                    throw new ApplicationException("Jasmine specs failed!");
                }
            }

            return true;
        }
    }
}