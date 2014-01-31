using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using FubuCore;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.OwinHost.Middleware;
using FubuMVC.OwinHost.Middleware.StaticFiles;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.OwinHost.Testing.Middleware
{
    [TestFixture]
    public class StaticFileMiddlewareTester
    {
        private OwinCurrentHttpRequest theRequest;
        private StubFubuApplicationFiles theFiles = new StubFubuApplicationFiles();
        private StaticFileMiddleware theMiddleware;
        private OwinHttpWriter theWriter;
 
        [SetUp]
        public void SetUp()
        {
            theRequest = new OwinCurrentHttpRequest();
            theWriter = new OwinHttpWriter(theRequest.Environment);
            theMiddleware = new StaticFileMiddleware(null, theFiles);
        }

        private void fileDoesNotExist(string path)
        {
            var file = theFiles.Find(path);
            if (file != null)
            {
                File.Delete(file.Path);
            }
        }

        private MiddlewareContinuation forMethodAndFile(string method, string path)
        {
            theRequest.HttpMethod(method);
            if (theRequest.Environment.ContainsKey(OwinConstants.RequestPathKey))
            {
                theRequest.Environment[OwinConstants.RequestPathKey] = path;
            }
            else
            {
                theRequest.Environment.Add(OwinConstants.RequestPathKey, path);
            }

            return theMiddleware.Invoke(theRequest, theWriter);
        }

        [Test]
        public void just_continue_if_not_GET_or_HEAD()
        {
            theFiles.WriteFile("anything.txt", "some content");

            forMethodAndFile("POST", "anything.txt").AssertOnlyContinuesToTheInner();
            forMethodAndFile("PUT", "anything.txt").AssertOnlyContinuesToTheInner();
        }

        [Test]
        public void GET_and_the_file_does_not_exist()
        {
            fileDoesNotExist("something.txt");

            forMethodAndFile("GET", "something.txt")
                .AssertOnlyContinuesToTheInner();
        }

        [Test]
        public void HEAD_and_the_file_does_not_exist()
        {
            fileDoesNotExist("something.txt");

            forMethodAndFile("HEAD", "something.txt")
                .AssertOnlyContinuesToTheInner();
        }

        [Test]
        public void file_exists_on_GET_request_no_headers_of_any_kind_should_write_file()
        {
            theFiles.WriteFile("/folder1/foo.txt", "hey you!");

            var continuation = forMethodAndFile("GET", "/folder1/foo.txt")
                .ShouldBeOfType<WriteFileContinuation>();

            continuation.File.RelativePath
                .ShouldEqual("folder1/foo.txt");
        }

        [Test]
        public void file_exists_on_HEAD_request_when_file_exists()
        {
            theFiles.WriteFile("foo.txt", "something grand");

            var continuation = forMethodAndFile("HEAD", "foo.txt")
                .ShouldBeOfType<WriteFileHeadContinuation>();

            continuation.Status.ShouldEqual(HttpStatusCode.OK);
            continuation.File.RelativePath.ShouldEqual("foo.txt");
        }
    }

    public class StubFubuApplicationFiles : IFubuApplicationFiles
    {
        public void WriteFile(string relativePath, string contents)
        {
            new FileSystem().WriteStringToFile(relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar).TrimStart('/'), contents);
        }

        public string GetApplicationPath()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<ContentFolder> AllFolders { get; private set; }
        public IEnumerable<IFubuFile> FindFiles(FileSet fileSet)
        {
            throw new System.NotImplementedException();
        }

        public IFubuFile Find(string relativeName)
        {
            var path = Environment.CurrentDirectory.AppendPath(relativeName.Replace('/', Path.DirectorySeparatorChar));
            
            return File.Exists(path)
                ? new FubuFile(path, "application")
                {
                    RelativePath = relativeName
                }
                : null;
        }
    }
}