using FubuCore;
using StoryTeller;
using StoryTeller.Engine;

namespace Serenity
{
    public class FubuMvcContext : IExecutionContext
    {
        private readonly FubuMvcSystem _system;

        public FubuMvcContext(FubuMvcSystem system)
        {
            _system = system;
        }

        public void Dispose()
        {
        }

        public IServiceLocator Services
        {
            get { return _system.Application.Services; }
        }

        public void AfterExecution(ISpecContext context)
        {
            // Nothing yet, but will be the logging
        }
    }
}