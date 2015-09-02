using System;
using FubuMVC.Core.Runtime.Files;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Runtime.Files
{
    [TestFixture]
    public class TrackedSetTester
    {
        private readonly StubFubuFile file1 = new StubFubuFile("foo1", DateTime.Now);
        private readonly StubFubuFile file2 = new StubFubuFile("foo2", DateTime.Now);
        private readonly StubFubuFile file3 = new StubFubuFile("foo3", DateTime.Now);
        private readonly StubFubuFile file4 = new StubFubuFile("foo4", DateTime.Now);
        private readonly StubFubuFile file5 = new StubFubuFile("foo5", DateTime.Now);
        private readonly StubFubuFile file6 = new StubFubuFile("foo6", DateTime.Now);
        private readonly StubFubuFile file7 = new StubFubuFile("foo7", DateTime.Now);

        [Test]
        public void store_files_and_test_for_changes_with_no_changes()
        {
            var files = new IFubuFile[] {file1, file2, file3, file4, file5, file6, file7};

            var set = new TrackedSet(files);

            set.DetectChanges(files).HasChanges().ShouldBeFalse();
        }

        [Test]
        public void find_deletes()
        {
            var files = new IFubuFile[] { file1, file2, file3, file4, file5, file6, file7 };

            var set = new TrackedSet(files);

            var files2 = new IFubuFile[] { file1, file2, file3, file4, file5 };

            set.DetectChanges(files2).Deleted.ShouldHaveTheSameElementsAs(file6.RelativePath, file7.RelativePath);

        }

        [Test]
        public void find_adds()
        {
            var files = new IFubuFile[] { file1, file2, file3, file4, file5 };

            var set = new TrackedSet(files);

            var files2 = new IFubuFile[] { file1, file2, file3, file4, file5, file6, file7 };

            set.DetectChanges(files2).Added.ShouldHaveTheSameElementsAs(file6, file7);
        }


    }
}