using FubuMVC.Core;
using FubuMVC.StructureMap;
using StructureMap;

namespace %NAMESPACE%
{
	public class %APPLICATION_SOURCE% : IApplicationSource
	{
	    public FubuApplication BuildApplication()
	    {
            return FubuApplication.For<%SHORT_NAME%FubuRegistry>()
				.StructureMap<%STRUCTUREMAP_REGISTRY%>();
	    }
	}
}