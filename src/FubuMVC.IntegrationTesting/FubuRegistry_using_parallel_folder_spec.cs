using FubuMVC.Core;
using Xunit;

namespace FubuMVC.IntegrationTesting
{
    
    public class FubuRegistry_using_parallel_folder_spec
    {
        [Fact]
        public void use_parallel_folder()
        {
            using (var runtime = FubuRuntime.Basic(_ => _.UseParallelDirectory("AssemblyPackage")))
            {
                runtime.Scenario(_ =>
                {
                    _.Get.Url("/JavaScript1.js");
                    _.StatusCodeShouldBeOk();

                    // There's a file in the AssemblyPackage project called JavaScript1.js
                    // with this line
                    _.ContentShouldContain("var answer = 42;");
                });
            }
        }
    }
}