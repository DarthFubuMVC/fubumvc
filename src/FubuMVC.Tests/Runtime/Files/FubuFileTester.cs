using System;
using System.IO;
using FubuMVC.Core.Runtime.Files;
using Xunit;
using Shouldly;
using Rhino.Mocks;
using FubuCore;

namespace FubuMVC.Tests.Runtime.Files
{
    
    public class FubuFileTester
    {
        [Fact]
        public void read_contents()
        {
            var file = new FubuFile(Path.Combine("Runtime", "Files", "Data", "a.txt"));
            file.ReadContents().Trim().ShouldBe("some text from a.txt");
        }

        [Fact]
        public void read_lines()
        {
            var file = new FubuFile(Path.Combine("Runtime", "Files", "Data", "a.txt"));
            var action = MockRepository.GenerateMock<Action<string>>();

            file.ReadLines(action);

            action.AssertWasCalled(x => x.Invoke("some text from a.txt"));
        }

        [Fact]
        public void read_contents_by_stream()
        {
            var wasCalled = false;
            var file = new FubuFile(Path.Combine("Runtime", "Files", "Data", "a.txt"));
            file.ReadContents(stream =>
            {
                wasCalled = true;
                stream.ReadAllText().ShouldBe("some text from a.txt");
            });

            wasCalled.ShouldBeTrue();
        }

        [Fact]
        public void length()
        {
            new FileSystem().WriteStringToFile("ghostbusters.txt", "Who you gonna call?");

            new FubuFile("ghostbusters.txt")
                .Length().ShouldBe(19);
        }

        [Fact]
        public void last_modified()
        {
            var now = DateTime.UtcNow;

            new FileSystem().WriteStringToFile("ghostbusters.txt", "Who you gonna call?");

            var lastModified = new FubuFile("ghostbusters.txt")
                .LastModified();


            (lastModified.ToFileTimeUtc() - now.ToFileTimeUtc())
                .ShouldBeLessThan(1);
        }

        [Fact]
        public void etag_is_predictable()
        {
            new FileSystem().WriteStringToFile("ghostbusters.txt", "Who you gonna call?");

            var etag1 = new FubuFile("ghostbusters.txt").Etag();
            var etag2 = new FubuFile("ghostbusters.txt").Etag();
            var etag3 = new FubuFile("ghostbusters.txt").Etag();

            etag1.ShouldBe(etag2);
            etag1.ShouldBe(etag3);
        }

        [Fact]
        public void etag_changes_on_file_changes()
        {
            new FileSystem().WriteStringToFile("ghostbusters.txt", "Who you gonna call?");

            var etag1 = new FubuFile("ghostbusters.txt").Etag();

            new FileSystem().WriteStringToFile("ghostbusters.txt", "He slimed me!");

            var etag2 = new FubuFile("ghostbusters.txt").Etag();

            etag1.ShouldNotBe(etag2);
        }

    }
}