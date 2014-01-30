using System;
using System.IO;
using FubuMVC.Core.Runtime.Files;
using NUnit.Framework;
using FubuTestingSupport;
using Rhino.Mocks;
using FubuCore;

namespace FubuMVC.Tests.Runtime.Files
{
    [TestFixture]
    public class FubuFileTester
    {
        [Test]
        public void read_contents()
        {
            var file = new FubuFile(Path.Combine("Runtime", "Files", "Data", "a.txt"), ContentFolder.Application);
            file.ReadContents().Trim().ShouldEqual("some text from a.txt");
        }

        [Test]
        public void read_lines()
        {
            var file = new FubuFile(Path.Combine("Runtime", "Files", "Data", "a.txt"), ContentFolder.Application);
            var action = MockRepository.GenerateMock<Action<string>>();

            file.ReadLines(action);

            action.AssertWasCalled(x => x.Invoke("some text from a.txt"));
        }

        [Test]
        public void read_contents_by_stream()
        {
            var wasCalled = false;
            var file = new FubuFile(Path.Combine("Runtime", "Files", "Data", "a.txt"), ContentFolder.Application);
            file.ReadContents(stream =>
            {
                wasCalled = true;
                stream.ReadAllText().ShouldEqual("some text from a.txt");
            });

            wasCalled.ShouldBeTrue();
        }

        [Test]
        public void length()
        {
            new FileSystem().WriteStringToFile("ghostbusters.txt", "Who you gonna call?");

            new FubuFile("ghostbusters.txt", "Application")
                .Length().ShouldEqual(19);
        }

        [Test]
        public void last_modified()
        {
            var now = DateTime.UtcNow;

            new FileSystem().WriteStringToFile("ghostbusters.txt", "Who you gonna call?");

            var lastModified = new FubuFile("ghostbusters.txt", "Application")
                .LastModified();


            (lastModified.ToFileTimeUtc() - now.ToFileTimeUtc())
                .ShouldBeLessThan(1);
        }

        [Test]
        public void etag_is_predictable()
        {
            new FileSystem().WriteStringToFile("ghostbusters.txt", "Who you gonna call?");

            var etag1 = new FubuFile("ghostbusters.txt", "Application").Etag();
            var etag2 = new FubuFile("ghostbusters.txt", "Application").Etag();
            var etag3 = new FubuFile("ghostbusters.txt", "Application").Etag();

            etag1.ShouldEqual(etag2);
            etag1.ShouldEqual(etag3);
        }

        [Test]
        public void etag_changes_on_file_changes()
        {
            new FileSystem().WriteStringToFile("ghostbusters.txt", "Who you gonna call?");

            var etag1 = new FubuFile("ghostbusters.txt", "Application").Etag();

            new FileSystem().WriteStringToFile("ghostbusters.txt", "He slimed me!");

            var etag2 = new FubuFile("ghostbusters.txt", "Application").Etag();

            etag1.ShouldNotEqual(etag2);
        }

    }
}