using Bottles.Services;
using System;

namespace %NAMESPACE%
{
	public class %SERVICE_NAME%Loader : IApplicationLoader, IDisposable
	{
		public IDisposable Load()
		{
			// Bootstrap your service code in this method
			// Returning an IDisposable here is just a way 
			// to allow BottleServiceRunner to cleanly
			// shut down the running application later
			return this;
		}
		
		public void Dispose()
		{
			// shut down your running code and clean
			// up resources
		}
	}
}