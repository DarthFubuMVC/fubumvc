using System;
using System.IO;
using System.Text;
using FubuCsProjFile;
using FubuCore;

namespace Fubu.Generation
{
    public static class ViewModelBuilder
    {
       
        public static readonly string NakedModel = @"namespace %NAMESPACE%
{
    public class %CLASS%
    {

    }
}
";

        public static readonly string UrlModel = @"using FubuMVC.Core;

namespace %NAMESPACE%
{
    [UrlPattern(%URL%)]
    public class %CLASS%
    {

    }
}
";
        

        public static CodeFile BuildCodeFile(ViewInput input, Location location)
        {
            var filename = Path.GetFileNameWithoutExtension(input.Name) + ".cs";

            var path = location.Project.ProjectDirectory.AppendPath(location.RelativePath, filename);

            var text = createText(input, location);
            new FileSystem().WriteStringToFile(path, text);

            var file = new CodeFile(location.RelativePath.AppendPath(filename));
            location.Project.Add(file);

            location.Project.Save();

            if (input.OpenFlag)
            {
                EditorLauncher.LaunchFile(path);
            }

            return file;
        }

        private static string createText(ViewInput input, Location location)
        {
            var template = input.UrlFlag.IsEmpty() ? NakedModel : UrlModel;

            var builder = new StringBuilder(template);
            builder.Replace("%NAMESPACE%", location.Namespace);
            builder.Replace("%CLASS%", input.Name);
            if (input.UrlFlag.IsNotEmpty())
            {
                builder.Replace("%URL%", "\"" + input.UrlFlag + "\"");
            }

            return builder.ToString();


        }
    }
}