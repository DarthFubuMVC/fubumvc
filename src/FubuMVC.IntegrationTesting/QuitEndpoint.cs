namespace FubuMVC.IntegrationTesting
{
    public class QuitEndpoint
    {
        public string get_quit()
        {
            TestHost.Finish.Set();

            return "Quitting";
        }
    }
}