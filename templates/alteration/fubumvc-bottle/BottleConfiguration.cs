using FubuMVC.Core;

namespace %NAMESPACE%
{
	public class %SHORT_NAME%Registry : FubuPackageRegistry
	{
		public %SHORT_NAME%Registry()
		{
			// The presence of this class by itself will allow you
			// to import Endpoint/Chains from this Bottle into the main
			// application
		
			// Register any custom FubuMVC policies, inclusions, or 
			// other FubuMVC configuration here for ONLY this bottle
			// Or leave as is to use the default conventions unchanged
		}
	}
	
	public class %SHORT_NAME%Extensions : IFubuRegistryExtension
	{
        public void Configure(FubuRegistry registry)
        {
            // Register any policies or services to be applied
			// to the global 
        }
	}
}