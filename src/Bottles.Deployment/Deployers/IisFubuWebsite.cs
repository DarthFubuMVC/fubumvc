namespace Bottles.Deployment.Deployers
{
    public class IisFubuWebsite : IDirective
    {

        public IisFubuWebsite()
        {
            Port = 80;
        }

        public string WebsiteName { get; set; }
        public string WebsitePath { get; set; }

        public string VDir { get; set; }

        public string AppDirectory { get; set; }
        public string AppPool { get; set; }
        public int Port { get; set; }

        //credentials
        public string Username { get; set; }
        public string Password { get; set; }
        public bool HasCredentials()
        {
            return !string.IsNullOrEmpty(Username);
        }



        //host bottle?
        public string MainBottle { get; set; }

        public string[] Bottles { get; set; }
    }
}