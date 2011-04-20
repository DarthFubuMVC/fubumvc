using System;
using System.Collections.Generic;
using Bottles;
using Bottles.Diagnostics;

namespace FubuMVC.Spark
{
    public class SparkActivator : IActivator 
    {
        // Inject ISparkViewEngine and assign viewfolder?
        public SparkActivator(/* */)
        {
            
        }

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            throw new NotImplementedException();
        }
    }
}