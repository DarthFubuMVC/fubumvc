namespace Fubu.Templating
{
    public interface ITemplateStep
    {
        string Describe(TemplatePlanContext context);
        void Execute(TemplatePlanContext context);
    }
}