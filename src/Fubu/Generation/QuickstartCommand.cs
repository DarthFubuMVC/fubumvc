using System;
using FubuCore.CommandLine;

namespace Fubu.Generation
{
    public class QuickstartInput
    {
        public NewCommandInput ToInput()
        {
            throw new NotImplementedException();

//            return new NewCommandInput
//            {
//                
//                SolutionName = SolutionName,
//                PreviewFlag = PreviewFlag,
//                RippleFlag = RippleFlag,
//                ShortNameFlag = ShortNameFlag,
//
//                OptionsFlag = new string[]{"spark"}, // needs to get smarter here
//                TestsFlag = true
//            };
        }
    }

    [CommandDescription("Stand up a full FubuMVC application structure with the full idiomatic stack")]
    public class QuickstartCommand : FubuCommand<QuickstartInput>
    {
        public override bool Execute(QuickstartInput input)
        {
            return new NewCommand().Execute(input.ToInput());
        }
    }
}