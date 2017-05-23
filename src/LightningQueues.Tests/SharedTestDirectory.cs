using System;
using System.IO;
using System.Threading;
using Xunit;

namespace LightningQueues.Tests
{
    public class SharedTestDirectory : IDisposable
    {
        private readonly string _testTempDir;

        public SharedTestDirectory()
        {
            var testProjectDir = Directory.GetCurrentDirectory();
            _testTempDir = Path.Combine(Directory.GetParent(testProjectDir).Parent.FullName, "TestData");
        }

        public void Dispose()
        {
            for (var i = 0; i < 3; ++i)
            {
                try
                {
                    Directory.Delete(_testTempDir, true);
                    break;
                }
                catch (Exception)
                {
                    //timing issues with environment close releasing files and deleting directory
                    Thread.Sleep(100);
                }
            }
        }

        public string CreateNewDirectoryForTest()
        {
            var path = Path.Combine(_testTempDir, Guid.NewGuid().ToString());
            Directory.CreateDirectory(path);
            return path;
        }
    }

    [CollectionDefinition("SharedTestDirectory")]
    public class SharedTestDirectoryCollection : ICollectionFixture<SharedTestDirectory>
    {
    }
}