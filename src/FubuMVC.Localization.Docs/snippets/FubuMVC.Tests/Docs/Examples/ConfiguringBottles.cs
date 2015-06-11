using System.Collections.Generic;
using Bottles;
using Bottles.Diagnostics;
using FubuMVC.Core;
using FubuMVC.StructureMap;

namespace FubuMVC.Tests.Docs.Examples
{


    // SAMPLE: bootstrapping-custom-bottles
    public class CustomPackageLoadingApplication : IApplicationSource
    {
        public FubuApplication BuildApplication()
        {
            return FubuApplication
                .DefaultPolicies()
                .StructureMap()
                .Packages(x => {
                    x.Loader(new CustomPackageLoader());
                    x.Activator(new SpecialStartupActivator());
                });
        }
    }

    // This class will run at startup time
    public class SpecialStartupActivator : IActivator
    {
        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            // Do something!
        }
    }

    public class CustomPackageLoader : IPackageLoader
    {
        public IEnumerable<IPackageInfo> Load(IPackageLog log)
        {
            // discover and load Bottles (IPackageInfo) in your
            // own special application way
            yield break;
        }
    }
    // ENDSAMPLE
}