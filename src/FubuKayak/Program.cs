using FubuMVC.OwinHost;
using FubuTestApplication;

namespace FubuKayak
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // TODO -- convert to FubuCommand's
            // How do they get at the application?
            // Have to spin up a separate AppDomain?  Ick-y

            var host = new FubuOwinHost(new OwinApplication(), new SchedulerDelegate());
            host.RunApplication(5500);
            
        }
    }
}