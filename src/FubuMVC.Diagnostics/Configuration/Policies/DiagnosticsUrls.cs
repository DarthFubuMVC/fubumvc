using FubuCore;

namespace FubuMVC.Diagnostics.Configuration.Policies
{
    public class DiagnosticsUrls
    {
        public const string ROOT = "_fubu";
        public const string PREFIX = "~/";

        public static string ToRelativeUrl(string path)
        {
            if(string.IsNullOrEmpty(path))
            {
                return null;
            }

            path = path.Replace(PREFIX, "{0}/".ToFormat(ROOT));
            if(path.EndsWith("/"))
            {
                return path.Substring(0, path.Length - 1);
            }

            return path;
        }
    }
}