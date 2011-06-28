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

            return path.Replace(PREFIX, "{0}/".ToFormat(ROOT));
        }
    }
}