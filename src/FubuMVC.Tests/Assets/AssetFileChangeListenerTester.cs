using System;
using FubuCore.Logging;
using FubuMVC.Core.Assets.Caching;
using FubuMVC.Core.Assets.Files;
using FubuTestingSupport;
using NUnit.Framework;
using StructureMap;
using Rhino.Mocks;

namespace FubuMVC.Tests.Assets
{
    [TestFixture]
    public class when_changing_a_file : InteractionContext<AssetFileChangeListener>
    {
        private AssetFile theFile;
        private ILogger innerLogger;

        protected override void beforeEach()
        {
            theFile = new AssetFile("some name"){
                FullPath = "some path"
            };

            innerLogger = MockRepository.GenerateMock<ILogger>();

            Services.Inject<ILogger>(new InteractionContextLogger(innerLogger));

            ClassUnderTest.Changed(theFile);
        }

        [Test]
        public void should_audit_the_asset_file_change()
        {
            innerLogger.AssertWasCalled(x => x.InfoMessage(new AssetFileChangeDetected{
                Name = theFile.Name,
                Fullpath = theFile.FullPath
            }));
        }

        [Test]
        public void delegates_to_the_asset_file_cache()
        {
            MockFor<IAssetContentCache>().AssertWasCalled(x => x.Changed(theFile));
        }
    }

    public class InteractionContextLogger : ILogger
    {
        private readonly ILogger _inner;

        public InteractionContextLogger(ILogger inner)
        {
            _inner = inner;
        }

        public void Debug(string message, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public void Info(string message, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public void Error(string message, Exception ex)
        {
            throw new NotImplementedException();
        }

        public void Error(object correlationId, string message, Exception ex)
        {
            throw new NotImplementedException();
        }

        public void Debug(Func<string> message)
        {
            throw new NotImplementedException();
        }

        public void Info(Func<string> message)
        {
            throw new NotImplementedException();
        }

        public void DebugMessage(LogTopic message)
        {
            throw new NotImplementedException();
        }

        public void InfoMessage(LogTopic message)
        {
            throw new NotImplementedException();
        }

        public void DebugMessage<T>(Func<T> message) where T : class, LogTopic
        {
            _inner.DebugMessage(message());
        }

        public void InfoMessage<T>(Func<T> message) where T : class, LogTopic
        {
            _inner.InfoMessage(message());
        }
    }
}