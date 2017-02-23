using System;
using FubuMVC.Core.Runtime.Files;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Runtime.Files
{
    
    public class ChangeSetTester
    {

        [Fact]
        public void has_changes_negative()
        {
            var set = new ChangeSet();

            set.HasChanges().ShouldBeFalse();
        }

        [Fact]
        public void has_changes_positive_with_any_changed()
        {
            var set = new ChangeSet();
            set.Changed.Add(new StubFubuFile("foo", DateTime.Now));

            set.HasChanges().ShouldBeTrue();
        }

        [Fact]
        public void has_changes_positive_with_any_deleted()
        {
            var set = new ChangeSet();
            set.Deleted.Add(new StubFubuFile("foo", DateTime.Now).RelativePath);

            set.HasChanges().ShouldBeTrue();
        }

        [Fact]
        public void has_changes_positive_with_any_added()
        {
            var set = new ChangeSet();
            set.Added.Add(new StubFubuFile("foo", DateTime.Now));

            set.HasChanges().ShouldBeTrue();
        }

    }
}