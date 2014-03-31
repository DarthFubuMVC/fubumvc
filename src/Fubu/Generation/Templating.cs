using System;
using System.IO;
using System.Linq;
using System.Reflection;
using FubuCore;
using FubuCsProjFile.Templating.Graph;
using FubuCsProjFile.Templating.Planning;
using FubuCsProjFile.Templating.Runtime;

namespace Fubu.Generation
{
    public static class Templating
    {
        private static readonly Lazy<TemplateLibrary> _templates;

        static Templating()
        {
            _templates = new Lazy<TemplateLibrary>(LoadTemplates);
        }

        public static TemplateLibrary Library
        {
            get
            {
                return _templates.Value;
            }
        }

        public static TemplateLibrary LoadTemplates()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory.AppendPath("templates");
            if (Directory.Exists(path))
            {
                return new TemplateLibrary(path);
            }

            // Testing mode.
            path = Assembly.GetExecutingAssembly().Location.ToFullPath()
                .ParentDirectory().ParentDirectory().ParentDirectory()
                .ParentDirectory().ParentDirectory()
                .AppendPath("templates");

            if (!Directory.Exists(path))
            {
                path = AppDomain.CurrentDomain.BaseDirectory.ParentDirectory()
                    .ParentDirectory()
                    .ParentDirectory()
                    .ParentDirectory()
                    .AppendPath("templates");
            }

            return new TemplateLibrary(path);
        }

        public static TemplatePlan BuildPlan(TemplateRequest request)
        {
            var planner = new TemplatePlanBuilder(_templates.Value);

            var plan = planner.BuildPlan(request);
            if (plan.Steps.OfType<GemReference>().Any())
            {
                plan.Add(new BundlerStep());
            }

            return plan;
        }

        public static void ExecutePlan(TemplatePlan plan)
        {
            plan.Execute();

            new RakeStep().Alter(plan);
            

            plan.WriteInstructions();
        }
    }
}