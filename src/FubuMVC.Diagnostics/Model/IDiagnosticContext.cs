using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Diagnostics.Model
{
    public interface IDiagnosticContext
    {
        DiagnosticGroup CurrentGroup();
        DiagnosticChain CurrentChain();
        string Title();
    }

    public class DiagnosticContext : IDiagnosticContext
    {
        private readonly ICurrentChain _currentChain;
        private readonly IFubuRequest _fubuRequest;
        private readonly DiagnosticGraph _graph;

        public DiagnosticContext(ICurrentChain currentChain, IFubuRequest fubuRequest, DiagnosticGraph graph)
        {
            _currentChain = currentChain;
            _fubuRequest = fubuRequest;
            _graph = graph;
        }

        public DiagnosticGroup CurrentGroup()
        {
            var current = CurrentChain();
            if (current != null) return current.Group;

            var request = _fubuRequest.Get<GroupRequest>();
            return _graph.FindGroup(request.Name);
        }

        public DiagnosticChain CurrentChain()
        {
            return _currentChain.OriginatingChain as DiagnosticChain;
        }

        public string Title()
        {
            var chain = CurrentChain();
            if (chain != null) return chain.Title;

            return CurrentGroup().Title;
        }
    }

    
}