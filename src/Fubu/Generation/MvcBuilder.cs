using System;
using Fubu.Generation.Templates;
using FubuCore;
using FubuCsProjFile;

namespace Fubu.Generation
{
    public static class MvcBuilder
    {
        public static void BuildView(ViewInput input)
        {
            Location location = ProjectFinder.DetermineLocation(Environment.CurrentDirectory);
            var template = FileTemplate.Find(location, input.TemplateFlag);

            ViewModelBuilder.BuildCodeFile(input, location);

            var modelName = location.Namespace + "." + input.Name;


            var path = ViewBuilder.Write(template, location, modelName);

            var viewPath = path.PathRelativeTo(location.ProjectFolder()).Replace('\\', '/');
            location.Project.Add(new Content(viewPath));
            location.Project.Save();

            if (input.OpenFlag)
            {
                EditorLauncher.LaunchFile(path);
            }
        }
    }
}