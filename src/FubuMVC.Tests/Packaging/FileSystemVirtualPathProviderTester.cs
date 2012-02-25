using FubuMVC.Core.Packaging.VirtualPaths;
using NUnit.Framework;

namespace FubuMVC.Tests.Packaging
{
    [TestFixture]
    public class FileSystemVirtualPathProviderTester
    {
        [Test]
        public void should_return_false_when_file_name_contains_invalid_characters()
        {
            var pathProvider = new FileSystemVirtualPathProvider();
            pathProvider.RegisterContentDirectory("/yada");
            pathProvider.FileExists("/yada\"");
        }
    }
}