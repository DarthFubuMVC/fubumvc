using FubuCore;
using FubuCore.Logging;
using FubuMVC.Core;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Runtime;
using FubuMVC.StructureMap;
using Rhino.Mocks;
using StructureMap;

namespace FubuMVC.Tests
{
    public class MockedFubuRequestContext : FubuRequestContext
    {
        public MockedFubuRequestContext(IContainer container)
            : base(
                new StructureMapServiceLocator(container), container.GetInstance<IHttpRequest>(), container.GetInstance<IFubuRequest>(),
                container.GetInstance<IOutputWriter>(), new RecordingLogger())
        {
            
        }

        public MockedFubuRequestContext()
            : base(new InMemoryServiceLocator(), OwinHttpRequest.ForTesting(), new InMemoryFubuRequest(), MockRepository.GenerateMock<IOutputWriter>(), new RecordingLogger())
        {
        }
    }
}