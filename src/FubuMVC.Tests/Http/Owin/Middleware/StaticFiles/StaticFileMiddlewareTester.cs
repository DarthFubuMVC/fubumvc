﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using FubuCore;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Http.Owin.Middleware;
using FubuMVC.Core.Http.Owin.Middleware.StaticFiles;
using FubuMVC.Core.Runtime.Files;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Http.Owin.Middleware.StaticFiles
{
    
    public class StaticFileMiddlewareTester
    {
        private OwinHttpRequest theRequest;
        private StubFubuApplicationFiles theFiles = new StubFubuApplicationFiles();
        private StaticFileMiddleware theMiddleware;
        private OwinHttpResponse theResponse;

        public StaticFileMiddlewareTester()
        {
            theRequest = new OwinHttpRequest();
            theResponse = new OwinHttpResponse(theRequest.Environment);


            theMiddleware = new StaticFileMiddleware(null, theFiles, new AssetSettings());
        }

        private void fileDoesNotExist(string path)
        {
            var file = theFiles.Find(path);
            if (file != null)
            {
                File.Delete(file.Path);
            }
        }

        private MiddlewareContinuation forMethodAndFile(string method, string path, string etag = null)
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

            return theMiddleware.Invoke(theRequest, theResponse);
        }

        [Fact]
        public void will_not_write_a_file_that_is_denied_even_if_it_exists()
        {
            theFiles.WriteFile("my.config", "some stuff");
        
            forMethodAndFile("GET", "my.config")
                .AssertOnlyContinuesToTheInner();
        }

        [Fact]
        public void just_continue_if_not_GET_or_HEAD()
        {
            theFiles.WriteFile("anything.htm", "some content");

            forMethodAndFile("POST", "anything.htm").AssertOnlyContinuesToTheInner();
            forMethodAndFile("PUT", "anything.htm").AssertOnlyContinuesToTheInner();
        }

        [Fact]
        public void GET_and_the_file_does_not_exist()
        {
            fileDoesNotExist("something.js");

            forMethodAndFile("GET", "something.js")
                .AssertOnlyContinuesToTheInner();
        }

        [Fact]
        public void HEAD_and_the_file_does_not_exist()
        {
            fileDoesNotExist("something.js");

            forMethodAndFile("HEAD", "something.js")
                .AssertOnlyContinuesToTheInner();
        }

        [Fact]
        public void file_exists_on_GET_request_no_headers_of_any_kind_should_write_file()
        {
            theFiles.WriteFile("/folder1/foo.htm", "hey you!");

            var continuation = forMethodAndFile("GET", "/folder1/foo.htm")
                .ShouldBeOfType<WriteFileContinuation>();

            continuation.File.RelativePath
                .ShouldBe("folder1/foo.htm");
        }

        [Fact]
        public void file_exists_on_HEAD_request_when_file_exists()
        {
            theFiles.WriteFile("foo.css", "something grand");

            var continuation = forMethodAndFile("HEAD", "foo.css")
                .ShouldBeOfType<WriteFileHeadContinuation>();

            continuation.Status.ShouldBe(HttpStatusCode.OK);
            continuation.File.RelativePath.ShouldBe("foo.css");
        }

        [Fact]
        public void file_exists_on_GET_but_miss_on_IfMatch_header()
        {
            var file = theFiles.WriteFile("foo.css", "some contents");
            var differentEtag = file.Etag() + "!"; // just to make it different

            theRequest.AppendHeader(HttpRequestHeaders.IfMatch, differentEtag);

            var continuation = forMethodAndFile("GET", "foo.css")
                .ShouldBeOfType<WriteStatusCodeContinuation>();

            continuation.Status.ShouldBe(HttpStatusCode.PreconditionFailed);
        }

        [Fact]
        public void file_exists_on_GET_and_hit_on_IfMatch_header()
        {
            var file = theFiles.WriteFile("foo.css", "some contents");

            theRequest.AppendHeader(HttpRequestHeaders.IfMatch, file.Etag());

            var continuation = forMethodAndFile("GET", "foo.css")
                .ShouldBeOfType<WriteFileContinuation>();

            continuation.File.RelativePath.ShouldBe("foo.css");
        }

        [Fact]
        public void file_exists_on_GET_but_miss_on_IfNoneMatch_header()
        {
            var file = theFiles.WriteFile("foo.css", "some contents");
            var differentEtag = file.Etag() + "!"; // just to make it different

            theRequest.AppendHeader(HttpRequestHeaders.IfNoneMatch, differentEtag);

            var continuation = forMethodAndFile("GET", "foo.css")
                .ShouldBeOfType<WriteFileContinuation>();

            continuation.File.RelativePath.ShouldBe("foo.css");
        }

        [Fact]
        public void file_exists_on_GET_and_hit_on_IfNoneMatch_header()
        {
            var file = theFiles.WriteFile("foo.css", "some contents");

            theRequest.AppendHeader(HttpRequestHeaders.IfNoneMatch, file.Etag());

            var continuation = forMethodAndFile("GET", "foo.css")
                .ShouldBeOfType<WriteFileHeadContinuation>();

            continuation.Status.ShouldBe(HttpStatusCode.NotModified);
            continuation.File.RelativePath.ShouldBe("foo.css");
        }

        [Fact]
        public void if_modified_since_is_false_on_a_good_request()
        {
            var file = theFiles.WriteFile("foo.css", "some contents");

            theRequest.IfModifiedSince(file.LastModified().AddDays(1));

            var continuation = forMethodAndFile("GET", "foo.css")
                .ShouldBeOfType<WriteFileHeadContinuation>();

            continuation.Status.ShouldBe(HttpStatusCode.NotModified);
            continuation.File.RelativePath.ShouldBe("foo.css");
        }

        [Fact]
        public void if_modified_since_is_true_on_a_good_request_should_write_the_file()
        {
            var file = theFiles.WriteFile("foo.css", "some contents");

            theRequest.IfModifiedSince(file.LastModified().AddDays(-1));

            var continuation = forMethodAndFile("GET", "foo.css")
                .ShouldBeOfType<WriteFileContinuation>();

            continuation.File.RelativePath.ShouldBe("foo.css");
        }

        [Fact]
        public void if_un_modified_value_is_satisfied_so_write_the_file()
        {
            var file = theFiles.WriteFile("foo.css", "some contents");

            // the file has not been modified since the header time
            theRequest.IfUnModifiedSince(file.LastModified().AddDays(1));

            var continuation = forMethodAndFile("GET", "foo.css")
                .ShouldBeOfType<WriteFileContinuation>();

            continuation.File.RelativePath.ShouldBe("foo.css");
        }

        [Fact]
        public void if_un_modified_value_is_not_satisfied_write_the_file_head_with_412()
        {
            var file = theFiles.WriteFile("foo.css", "some contents");

            // the file *has* been modified since the header time
            theRequest.IfUnModifiedSince(file.LastModified().AddDays(-1));

            var continuation = forMethodAndFile("GET", "foo.css")
                .ShouldBeOfType<WriteStatusCodeContinuation>();

            continuation.Status.ShouldBe(HttpStatusCode.PreconditionFailed);
        }
    }

    public class StubFubuApplicationFiles : IFubuApplicationFiles
    {
        public FubuFile WriteFile(string relativePath, string contents)
        {
            var path = relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar).TrimStart('/');
            new FileSystem().WriteStringToFile(path, contents);
        
            return new FubuFile(path);
        }

        public string RootPath
        {
            get { throw new System.NotImplementedException(); }
        }

        public IEnumerable<IFubuFile> FindFiles(FileSet fileSet)
        {
            throw new System.NotImplementedException();
        }

        public IFubuFile Find(string relativeName)
        {
            var path = System.Environment.CurrentDirectory.AppendPath(relativeName.Replace('/', Path.DirectorySeparatorChar));
            
            return File.Exists(path)
                ? new FubuFile(path)
                {
                    RelativePath = relativeName
                }
                : null;
        }
    }
}