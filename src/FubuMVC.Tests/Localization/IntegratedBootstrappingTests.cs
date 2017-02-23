using System.Diagnostics;
using FubuMVC.Core;
using Xunit;

namespace FubuMVC.Tests.Localization
{
    
    public class IntegratedBootstrappingTests
    {
        [Fact]
        public void smoke()
        {
            using (FubuRuntime.Basic(_ => _.Features.Localization.Enable(true)))
            {
                Debug.WriteLine("Ok, can load localization just fine");  
            } 
        }
    }
}