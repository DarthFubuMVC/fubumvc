using System;
using System.Collections.Generic;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Assets.Http;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;

namespace FubuMVC.Tests.Assets.Http
{
    [TestFixture]
    public class when_writing_an_image : InteractionContext<ContentWriter>
    {
        private string[] theContentPathArguments;

        protected override void beforeEach()
        {
            theContentPathArguments = new string[]{"images", "icon.gif"};

            // Precondition here
            new AssetPath(theContentPathArguments).IsImage().ShouldBeTrue();
        
            ClassUnderTest.WriteContent(theContentPathArguments);
        }

        [Test]
        public void should_have_delegated_to_the_image_writer()
        {
            var expectedName = new AssetPath(theContentPathArguments).ToFullName();
            MockFor<IImageWriter>().AssertWasCalled(x => x.WriteImageToOutput(expectedName));
        }
    }

    [TestFixture]
    public class when_writing_textual_output : InteractionContext<ContentWriter>
    {
        private string[] theContentPathArguments;
        private AssetFile[] theFiles;
        private const string theContent = "blah blah blah";
        private RecordingResponseCache theCache;

        protected override void beforeEach()
        {
            theContentPathArguments = new string[] { "scripts", "combo1.js" };
            var assetPath = new AssetPath(theContentPathArguments);
            assetPath.IsImage().ShouldBeFalse();

            theFiles = new AssetFile[]{
                new AssetFile("script1.js"){FullPath = "1.js"},
                new AssetFile("script2.js"){FullPath = "2.js"},
                new AssetFile("script3.js"){FullPath = "3.js"},
                new AssetFile("script4.js"){FullPath = "4.js"},
            };

            theCache = new RecordingResponseCache();
            Services.Inject<IResponseCaching>(theCache);

            Services.Inject<IContentPlanExecutor>(new StubContentPlanExecutor(assetPath, theContent, theFiles));

            ClassUnderTest.WriteContent(theContentPathArguments);
        }

        [Test]
        public void should_write_the_files_to_the_caching()
        {
            theCache.LocalFiles.ShouldHaveTheSameElementsAs(theFiles.Select(a => a.FullPath));
        }

        [Test]
        public void should_write_out_the_contents()
        {
            MockFor<IOutputWriter>().AssertWasCalled(x => x.Write(MimeType.Javascript, theContent));
        }


    }

    public class RecordingResponseCache : IResponseCaching
    {
        public void CacheRequestAgainstFileChanges(IEnumerable<string> localFiles)
        {
            LocalFiles = localFiles;
        }

        public IEnumerable<string> LocalFiles { get; set; }

        public void CacheRequestAgainstFileChanges(string file)
        {
            throw new NotImplementedException();
        }
    } 

    public class StubContentPlanExecutor : IContentPlanExecutor
    {
        private readonly AssetPath _path;
        private readonly string _contents;
        private readonly IEnumerable<AssetFile> _files;

        public StubContentPlanExecutor(AssetPath path, string contents, IEnumerable<AssetFile> files)
        {
            _path = path;
            _contents = contents;
            _files = files;
        }

        public void Execute(AssetPath path, ProcessContentAction continuation)
        {
            path.ShouldEqual(_path);

            continuation(_contents, _files);
        }

        public string Contents
        {
            get { return _contents; }
        }
    }
}