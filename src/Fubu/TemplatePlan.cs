using System.Collections.Generic;

namespace Fubu
{
    public class TemplatePlan
    {
        private readonly IList<ITemplateStep> _steps = new List<ITemplateStep>();

        public void AddStep(ITemplateStep step)
        {
            _steps.Add(step);
        }

        public IEnumerable<ITemplateStep> Steps { get { return _steps; } }
    }

    public interface ITemplateStep
    {
        void Execute(TemplatePlanContext context);
    }

    public class TemplatePlanContext
    {
        public string TargetPath { get; set; }
        public NewCommandInput Input { get; set; }
    }

    public interface ITemplatePlanExecutor
    {
        void Execute(TemplatePlan plan);
    }
}