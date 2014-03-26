using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Diagnostics.Model
{
    public class DiagnosticGraph
    {
        private readonly IList<DiagnosticGroup> _groups = new List<DiagnosticGroup>();

        public void Add(Assembly assembly)
        {
            var calls = DiagnosticGroup.FindCalls(assembly);
            if (calls.Any())
            {
                var group = new DiagnosticGroup(assembly, calls);
                _groups.Add(group);
            }
        }

        public DiagnosticGroup FindGroup(string name)
        {
            return _groups.FirstOrDefault(x => x.Name.EqualsIgnoreCase(name)) ??
                   _groups.FirstOrDefault(x => x.Url.EqualsIgnoreCase(name));
        }

        public IEnumerable<DiagnosticGroup> Groups()
        {
            return _groups;
        }
    }
}