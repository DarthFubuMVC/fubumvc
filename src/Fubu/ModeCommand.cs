using System;
using System.ComponentModel;
using FubuCore.CommandLine;
using FubuMVC.Core;
using FubuCore;

namespace Fubu
{
    public class ModeInput
    {
        [Description("Named mode for modes other than Development")]
        public string ModeFlag { get; set; }

        [Description("Sets the environment variable to FubuMode=Development")]
        public bool DevFlag { get; set; }

        [Description("Clears the 'FubuMode' environment variable")]
        public bool ClearFlag { get; set; }
    }

    [CommandDescription("Reads and sets the 'FubuMode' environment variable")]
    public class ModeCommand : FubuCommand<ModeInput>
    {
        public override bool Execute(ModeInput input)
        {
            if (input.ClearFlag)
            {
                FubuMode.Detector.SetMode(string.Empty);
            }

            if (input.DevFlag)
            {
                FubuMode.Detector.SetMode(FubuMode.Development);
            }

            if (input.ModeFlag.IsNotEmpty())
            {
                FubuMode.Detector.SetMode(input.ModeFlag);
            }

            FubuMode.Reset();

            var mode = FubuMode.Mode();
            if (mode.IsEmpty())
            {
                Console.WriteLine("No FubuMode is set");
            }
            else
            {
                Console.WriteLine("FubuMode = '{0}'", FubuMode.Mode());
            }

            return true;
        }
    }
}