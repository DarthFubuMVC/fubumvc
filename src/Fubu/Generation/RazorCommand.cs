using FubuCore.CommandLine;

namespace Fubu.Generation
{
    [CommandDescription("Creates and attaches a Razor view model/view pair to the project in the current folder")]
    public class RazorCommand : FubuCommand<ViewInput>
    {
        public override bool Execute(ViewInput input)
        {
            input.TemplateFlag = input.TemplateFlag ?? "view.cshtml";

            MvcBuilder.BuildView(input);

            return true;
        }
    }
}