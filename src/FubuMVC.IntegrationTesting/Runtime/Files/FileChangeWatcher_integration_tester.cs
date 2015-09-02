using System;
using System.IO;
using System.Linq;
using System.Threading;
using FubuCore;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.Services;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.IntegrationTesting.Runtime.Files
{
    [TestFixture, Explicit("Too slow to be in the CI build")]
    public class FileChangeWatcher_integration_tester : IChangeSetHandler
    {
        private string theDirectory;
        private ChangeSet LastChanges;

        [SetUp]
        public void SetUp()
        {
            LastChanges = null;

            theDirectory = Guid.NewGuid().ToString().ToFullPath();
            Directory.CreateDirectory(theDirectory);
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                Directory.Delete(theDirectory);
            }
            catch (Exception)
            {
                // don't blow up on this
            }
        }

        private void write(string file, string contents)
        {
            new FileSystem().WriteStringToFile(theDirectory.AppendPath(file), contents);
        }

        [Test]
        public void see_adds()
        {
            write("foo.txt", "1");
            write("bar.txt", "2");
            write("baz.txt", "3");

            using (var watcher = new FileChangeWatcher(theDirectory, FileSet.Shallow("*.txt"), this))
            {
                watcher.Start();

                write("new.txt", "I am new");

                Wait.Until(() => LastChanges != null).ShouldBeTrue();

                LastChanges.Added.Single().RelativePath.ShouldBe("new.txt");
            }
        }

        [Test]
        public void see_changes()
        {
            write("foo.txt", "1");
            write("bar.txt", "2");
            write("baz.txt", "3");

            using (var watcher = new FileChangeWatcher(theDirectory, FileSet.Shallow("*.txt"), this))
            {
                watcher.Start();

                write("foo.txt", "different");
                write("baz.txt", "different");

                Wait.Until(() => LastChanges != null).ShouldBeTrue();

                LastChanges.Changed.OrderBy(x => x.RelativePath).Select(x => x.RelativePath)
                    .ShouldHaveTheSameElementsAs("baz.txt", "foo.txt");
            }
        }

        [Test]
        public void no_changes()
        {
            write("foo.txt", "1");
            write("bar.txt", "2");
            write("baz.txt", "3");

            using (var watcher = new FileChangeWatcher(theDirectory, FileSet.Shallow("*.txt"), this))
            {
                watcher.Start();

                Thread.Sleep(2000);

                LastChanges.ShouldBeNull();
            }
        }

        [Test]
        public void see_deletes()
        {
            write("foo.txt", "1");
            write("bar.txt", "2");
            write("baz.txt", "3");

            using (var watcher = new FileChangeWatcher(theDirectory, FileSet.Shallow("*.txt"), this))
            {
                watcher.Start();

                File.Delete(theDirectory.AppendPath("foo.txt"));
                File.Delete(theDirectory.AppendPath("baz.txt"));

                Wait.Until(() => LastChanges != null).ShouldBeTrue();

                LastChanges.Deleted.OrderBy(x => x)
                    .ShouldHaveTheSameElementsAs("baz.txt", "foo.txt");
            }
        }

        void IChangeSetHandler.Handle(ChangeSet changes)
        {
            LastChanges = changes;
        }
    }
}