using FubuMVC.Core;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting
{
    [TestFixture]
    public class FubuRegistry_using_parallel_folder_spec
    {
        [Test]
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