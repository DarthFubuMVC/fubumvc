using StructureMap.Configuration.DSL;

namespace %NAMESPACE%
{
	public class %STRUCTUREMAP_REGISTRY% : Registry
	{
		public %STRUCTUREMAP_REGISTRY%()
		{
			// make any StructureMap configuration here
			
            // Sets up the default "IFoo is Foo" naming convention
            // for auto-registration within this assembly
            Scan(x => {
                x.TheCallingAssembly();
                x.WithDefaultConventions();
            });
		}
	}
}