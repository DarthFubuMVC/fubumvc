using System;
using FubuCore;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuMVC.Core.Environment;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Environment
{
    [TestFixture]
    public class FileExistsTester
    {
        [Test]
        public void positive_test()
        {
            new FileSystem().WriteStringToFile("file.txt", "anything");

            var log = new ActivationLog();

            var requirement = new FileExists("file.txt");

            requirement.Check(log);

            log.Success.ShouldBeTrue();
            log.FullTraceText().ShouldContain("File 'file.txt' exists");
        }

        [Test]
        public void negative_test()
        {
            var log = new ActivationLog();

            var file = Guid.NewGuid().ToString() + ".txt";
            var requirement = new FileExists(file);

            requirement.Check(log);

            log.Success.ShouldBeFalse();
            log.FullTraceText().ShouldContain("File '{0}' does not exist!".ToFormat(file));
        }

        [Test]
        public void positive_test_with_generic()
        {
            new FileSystem().WriteStringToFile("file.txt", "anything");
            var settings = new FileSettings
            {
                File = "file.txt"
            };

            var log = new ActivationLog();

            var requirement = new FileExists<FileSettings>(x => x.File, settings);

            requirement.Check(log);

            log.Success.ShouldBeTrue();
            log.FullTraceText().ShouldContain("File 'file.txt' defined by FileSettings.File exists");
        }

        [Test]
        public void negative_test_with_settings()
        {
            var log = new ActivationLog();

            var file = Guid.NewGuid().ToString() + ".txt";
            var settings = new FileSettings
            {
                File = file
            };

            var requirement = new FileExists<FileSettings>(x => x.File, settings);


            requirement.Check(log);

            log.Success.ShouldBeFalse();
            log.FullTraceText().ShouldContain("File '{0}' defined by FileSettings.File does not exist!".ToFormat(file));
        }

    }

    public class FileSettings
    {
        public string File { get; set; }
        public string Folder { get; set; }
    }
}