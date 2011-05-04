using Bottles.Configuration;
using FubuCore;
using Bottles;

namespace Bottles.Deployment.Directives
{
    public class FubuWebsite : IDirective
    {

        public static readonly string WEBSITE_NAME = "website-name";
        public static readonly string VIRTUAL_DIR = "virtual-dir";
        public static readonly string APP_POOL = "app-pool";

        public FubuWebsite()
        {
            Port = 80;
            DirectoryBrowsing = Activation.Disable;
            AnonAuth = Activation.Enable;
            BasicAuth = Activation.Disable;
            WindowsAuth = Activation.Disable;

            WebsitePhysicalPath = FileSystem.Combine(EnvironmentSettings.ROOT.ToSubstitution(), "web");
            VDir = VIRTUAL_DIR.ToSubstitution();
            VDirPhysicalPath = FileSystem.Combine(WebsitePhysicalPath, VDir);

            WebsiteName = WEBSITE_NAME.ToSubstitution();


            AppPool = APP_POOL.ToSubstitution();
        }

        /// <summary>
        /// IIS website name
        /// </summary>
        public string WebsiteName { get; set; }
        
        /// <summary>
        /// Path on disk to the Website
        /// </summary>
        public string WebsitePhysicalPath { get; set; }

        /// <summary>
        /// Application virtual directory in IIS
        /// </summary>
        public string VDir { get; set; }

        /// <summary>
        /// Physical path to virtual directory
        /// </summary>
        public string VDirPhysicalPath { get; set; }

        public string AppPool { get; set; }
        public int Port { get; set; }


        //credentials
        public string Username { get; set; }
        public string Password { get; set; }
        public bool HasCredentials()
        {
            return !string.IsNullOrEmpty(Username);
        }

        //iis options
        public Activation DirectoryBrowsing { get; set; }
        public Activation AnonAuth { get; set; }
        public Activation BasicAuth { get; set; }
        public Activation WindowsAuth { get; set; }
    }
}