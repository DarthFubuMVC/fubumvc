using System;
using FubuMVC.Core;
using FubuMVC.Core.StructureMap;

namespace SerenityDemonstrator
{
    public class DemonstratorApplication : IApplicationSource
    {
        public FubuApplication BuildApplication(string directory)
        {
            FubuMode.SetUpForDevelopmentMode();
            
            throw new Exception("NWO");
            //return FubuRuntime.Basic();
        }
    }
}