using FubuCsProjFile;
using FubuCsProjFile.Templating.Graph;

namespace Fubu.Generation
{
    public static class GenerationExtensions
    {
        public static void AddMatchingTestingProject(this TemplateRequest request, ProjectRequest project)
        {
            var testing = new ProjectRequest(project.Name + ".Testing", "baseline",
                                                 project.Name);

            testing.Alterations.Add("unit-testing");
            testing.OriginalProject = project.Name;
            testing.Version = project.Version ?? DotNetVersion.V40;

            request.AddTestingRequest(testing);
        }
    }
}