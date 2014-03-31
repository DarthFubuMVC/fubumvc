using FubuCore.CommandLine;

namespace Fubu.Generation
{
    [CommandDescription("Creates and attaches a Spark view model/view pair to the project in the current folder")]
    public class SparkCommand : FubuCommand<ViewInput>
    {
        public override bool Execute(ViewInput input)
        {
            input.TemplateFlag = input.TemplateFlag ?? "view.spark";

            MvcBuilder.BuildView(input);

            return true;
        }
    }
}