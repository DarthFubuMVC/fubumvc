using System;
using FubuCore;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuMVC.Core.Environment;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Environment
{
    
    public class FolderExistsTester
    {
        [Fact]
        public void positive_test()
        {
            new FileSystem().CreateDirectory("foo");

            var log = new ActivationLog();

            var requirement = new FolderExists("foo");

            requirement.Check(log);

            log.Success.ShouldBeTrue();
            log.FullTraceText().ShouldContain("Folder 'foo' exists");
        }

        [Fact]
        public void negative_test()
        {
            var log = new ActivationLog();

            var folder = Guid.NewGuid().ToString();
            var requirement = new FolderExists(folder);

            requirement.Check(log);

            log.Success.ShouldBeFalse();
            log.FullTraceText().ShouldContain("Folder '{0}' does not exist!".ToFormat(folder));
        }

        [Fact]
        public void positive_test_with_generic()
        {
            new FileSystem().CreateDirectory("foo");
            var settings = new FileSettings
            {
                Folder = "foo"
            };

            var log = new ActivationLog();

            var requirement = new FolderExists<FileSettings>(x => x.Folder, settings);

            requirement.Check(log);

            log.Success.ShouldBeTrue();
            log.FullTraceText().ShouldContain("Folder 'foo' defined by FileSettings.Folder exists");
        }

        [Fact]
        public void negative_test_with_settings()
        {
            var log = new ActivationLog();

            var folder = Guid.NewGuid().ToString();
            var settings = new FileSettings
            {
                Folder = folder
            };

            var requirement = new FolderExists<FileSettings>(x => x.Folder, settings);


            requirement.Check(log);

            log.Success.ShouldBeFalse();
            log.FullTraceText().ShouldContain("Folder '{0}' defined by FileSettings.Folder does not exist!".ToFormat(folder));
        }
    }
}