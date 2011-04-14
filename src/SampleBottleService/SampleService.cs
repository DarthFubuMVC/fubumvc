using System.Collections.Generic;
using Bottles;
using Bottles.Diagnostics;
using Bottles.Services;

namespace SampleBottleService
{
    public class SampleService :
        IBottleAwareService
    {
        public IEnumerable<IActivator> Bootstrap(IPackageLog log)
        {
            //boot up IOC 

            yield return new TestActivator();
        }

        public void Stop()
        {
            //MEH
        }
    }
}
