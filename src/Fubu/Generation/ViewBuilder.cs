using Fubu.Generation.Templates;
using FubuCore;
using System.Linq;
using FubuCsProjFile.Templating.Graph;

namespace Fubu.Generation
{
    public static class ViewBuilder
    {
        public static string Write(FileTemplate template, Location location, string inputModel)
        {
            var substitutions = new Substitutions();
            substitutions.Set("%MODEL%", inputModel);

            var path = location.CurrentFolder.AppendPath(inputModel.Split('.').Last() + template.Extension);
            var contents = template.Contents(substitutions);

            new FileSystem().WriteStringToFile(path, contents);

            return path;
        }
    }
}