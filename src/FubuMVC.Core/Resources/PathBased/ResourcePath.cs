using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Resources.PathBased
{
    public class ResourcePath : IMakeMyOwnUrl
    {
        private readonly string _path;

        public ResourcePath(string path)
        {
            _path = path;
        }

        public string Path
        {
            get { return _path; }
        }

        public string ToUrlPart()
        {
            return _path;
        }
    }
}