using FubuMVC.Core.Registration;

namespace FubuMVC.Core.Security.AntiForgery
{
    public class AntiForgeryServiceRegistry : ServiceRegistry
    {
        public AntiForgeryServiceRegistry()
        {
            SetServiceIfNone<IAntiForgeryValidator, AntiForgeryValidator>();
            SetServiceIfNone<IAntiForgeryEncoder, MachineKeyAntiForgeryEncoder>();
            SetServiceIfNone<IAntiForgerySerializer, BinaryAntiForgerySerializer>();
            SetServiceIfNone<IAntiForgeryTokenProvider, AntiForgeryTokenProvider>();
            SetServiceIfNone<IAntiForgeryService, AntiForgeryService>();
        }
    }
}