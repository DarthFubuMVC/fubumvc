using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Binding;
using HtmlTags;
using StoryTeller.Engine;

namespace Serenity
{
    public class FubuMvcContext : IExecutionContext, IResultsExtension
    {
        private readonly IFubuMvcSystem _system;
        private readonly IEnumerable<IContextualInfoProvider> _contextualProviders;

        public FubuMvcContext(IFubuMvcSystem system)
        {
            _system = system;

            _contextualProviders = system.ContextualProviders ?? new IContextualInfoProvider[0];
            _contextualProviders.Each(x => x.Reset());
        }

        public void Dispose()
        {

        }

        public IServiceLocator Services
        {
            get { return _system.Application.Services; }
        }

        public BindingRegistry BindingRegistry
        {
            get { return _system.Binding; }
        }

        public IEnumerable<HtmlTag> Tags()
        {
            return _contextualProviders.SelectMany(x => x.GenerateReports());
        }
    }
}