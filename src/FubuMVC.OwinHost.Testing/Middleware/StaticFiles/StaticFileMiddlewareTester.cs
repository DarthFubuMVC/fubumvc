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

namespace FubuMVC.OwinHost.Testing.Middleware.StaticFiles
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
            theMiddleware = new StaticFileMiddleware(null, theFiles, new OwinSettings());
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
        public void will_not_write_a_file_that_is_denied_even_if_it_exists()
        {
            theFiles.WriteFile("my.config", "some stuff");
        
            forMethodAndFile("GET", "my.config")
                .AssertOnlyContinuesToTheInner();
        }

        [Test]
        public void just_continue_if_not_GET_or_HEAD()
        {
            theFiles.WriteFile("anything.htm", "some content");

            forMethodAndFile("POST", "anything.htm").AssertOnlyContinuesToTheInner();
            forMethodAndFile("PUT", "anything.htm").AssertOnlyContinuesToTheInner();
        }

        [Test]
        public void GET_and_the_file_does_not_exist()
        {
            fileDoesNotExist("something.js");

            forMethodAndFile("GET", "something.js")
                .AssertOnlyContinuesToTheInner();
        }

        [Test]
        public void HEAD_and_the_file_does_not_exist()
        {
            fileDoesNotExist("something.js");

            forMethodAndFile("HEAD", "something.js")
                .AssertOnlyContinuesToTheInner();
        }

        [Test]
        public void file_exists_on_GET_request_no_headers_of_any_kind_should_write_file()
        {
            theFiles.WriteFile("/folder1/foo.htm", "hey you!");

            var continuation = forMethodAndFile("GET", "/folder1/foo.htm")
                .ShouldBeOfType<WriteFileContinuation>();

            continuation.File.RelativePath
                .ShouldEqual("folder1/foo.htm");
        }

        [Test]
        public void file_exists_on_HEAD_request_when_file_exists()
        {
            theFiles.WriteFile("foo.css", "something grand");

            var continuation = forMethodAndFile("HEAD", "foo.css")
                .ShouldBeOfType<WriteFileHeadContinuation>();

            continuation.Status.ShouldEqual(HttpStatusCode.OK);
            continuation.File.RelativePath.ShouldEqual("foo.css");
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