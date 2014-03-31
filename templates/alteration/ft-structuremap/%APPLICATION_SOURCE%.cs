using FubuMVC.Core;
using FubuMVC.StructureMap;
using FubuTransportation.Configuration;
using StructureMap;

namespace %NAMESPACE%
{
	public class %APPLICATION_SOURCE% : IApplicationSource
	{
	    public FubuApplication BuildApplication()
	    {
            return FubuTransport.For<%TRANSPORT_REGISTRY%>()
				.StructureMap<%STRUCTUREMAP_REGISTRY%>();
	    }
	}
}