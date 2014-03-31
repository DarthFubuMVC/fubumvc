using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fubu.Generation;
using FubuCore;
using FubuCsProjFile;
using FubuTestingSupport;
using NUnit.Framework;

namespace fubu.Testing.Generation
{
    
    public abstract class GenerationContext
    {
        private string _original;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _original = Environment.CurrentDirectory.ToFullPath();
            new FileSystem().DeleteDirectory("integrated");
            new FileSystem().CreateDirectory("integrated");

            RemoteOperations.Enabled = false;

            Environment.CurrentDirectory = "integrated";

            theContext();
        }

        [TestFixtureTearDown]
        public void Teardown()
        {
            Environment.CurrentDirectory = _original;
            RemoteOperations.Enabled = true;

            new FileSystem().DeleteDirectory("integrated");
        }

        protected abstract void theContext();

        public void assertDirectoryExists(params string[] parts)
        {
            Directory.Exists(Path.Combine(parts)).ShouldBeTrue();
        }

        public void assertFileExists(params string[] parts)
        {
            File.Exists(Path.Combine(parts)).ShouldBeTrue();
        }
        
        public CsProjFile csprojFor(string name)
        {
            var files = new FileSystem().FindFiles(Environment.CurrentDirectory, FileSet.Deep(name + ".csproj"));
        
            if (files.Count() == 0)
            {
                Assert.Fail("Project {0} could not be found".ToFormat(name));
            }

            return CsProjFile.LoadFrom(files.Single());
        }

        protected IList<string> readFile(params string[] parts)
        {
            assertFileExists(parts);

            return new FileSystem().ReadStringFromFile(Path.Combine(parts)).ReadLines().ToList();
        }
    }
}