using System;
using FubuCore;
using FubuCore.CommandLine;
using System.Linq;
using System.Collections.Generic;

namespace Serenity.Jasmine
{
    [CommandDescription("Opens up a web browser application to browse and execute Jasmine specifications",
        Name = "jasmine")]
    [Usage("default", "runs the jasmine tests")]
    [Usage("add_folders", "adds new folders to a Serenity/Jasmine project")]
    public class JasmineCommand : FubuCommand<JasmineInput>
    {
        public override bool Execute(JasmineInput input)
        {
            if (input.Mode == JasmineMode.add_folders)
            {
                new FileSystem().AlterFlatFile(input.SerenityFile, list =>
                {
                    var includes = input.Folders.Select(folder => "include:" + folder);
                    list.Fill(includes);
                });

                return true;
            }

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
                    throw new ApplicationException("Jasmine specs failed!");
                }
            }



            return true;
        }
    }
}