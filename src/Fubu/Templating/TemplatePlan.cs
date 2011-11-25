using System;
using System.Collections.Generic;
using FubuCore;

namespace Fubu.Templating
{
    public class TemplatePlan
    {
        private readonly IList<ITemplateStep> _steps = new List<ITemplateStep>();

        public void AddStep(ITemplateStep step)
        {
            _steps.Add(step);
        }

        public void Preview(TemplatePlanContext context)
        {
            Console.WriteLine("Template plan for {0}", context.Input.ProjectName);
            Steps
                .Each(step =>
                          {
                              var description = step.Describe(context);
                              if(description.IsNotEmpty())
                              {
                                  Console.WriteLine("* {0}", description);
                              }
                          });
        }

        public IEnumerable<ITemplateStep> Steps { get { return _steps; } }
    }
}