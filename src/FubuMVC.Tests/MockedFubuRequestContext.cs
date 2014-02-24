using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Http;
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
                new StructureMapServiceLocator(container), new StandInCurrentHttpRequest(), container.GetInstance<IFubuRequest>(),
                container.GetInstance<IOutputWriter>())
        {
            
        }

        public MockedFubuRequestContext() : base(new InMemoryServiceLocator(), new StandInCurrentHttpRequest(), new InMemoryFubuRequest(), MockRepository.GenerateMock<IOutputWriter>())
        {
        }
    }
}