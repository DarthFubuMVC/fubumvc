using FubuCore;
using Microsoft.Practices.ServiceLocation;

namespace FubuFastPack.Binding
{
    public class FastPackObjectConverter : ServiceEnabledObjectConverter
    {
        public FastPackObjectConverter(IServiceLocator locator) : base(locator)
        {
            RegisterConverterFamily<DomainEntityConverterFamily>();
        }
    }
}