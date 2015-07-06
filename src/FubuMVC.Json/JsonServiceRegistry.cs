using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Json
{
    public class JsonServiceRegistry : ServiceRegistry
    {
        public JsonServiceRegistry()
        {
            SetServiceIfNone<IJsonSerializer, NewtonSoftJsonSerializer>();
        }
    }
}