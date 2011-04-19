using System.Collections.Generic;

namespace Bottles.DependencyAnalysis
{
    public class InstallContext
    {
        public IList<Bottle> Bottles { get; private set; }
        public void RegisterBottleForInstall(Bottle bottle)
        {
            
        }

    }
}