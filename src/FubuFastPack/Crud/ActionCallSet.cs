using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;

namespace FubuFastPack.Crud
{
    public class ActionCallSet
    {
        private readonly IList<ActionCall> _calls = new List<ActionCall>();

        public ActionCallSet(IEnumerable<ActionCall> calls)
        {
            _calls.AddRange(calls.ToArray());
        }

        public ActionCall ByName(string name)
        {
            return _calls.FirstOrDefault(x => x.Method.Name == name);
        }

        public IEnumerable<ActionCall> StartingWith(string prefix)
        {
            return _calls.Where(x => x.Method.Name.StartsWith(prefix));
        }

        public IEnumerable<ActionCall> ForOutput(Func<Type, bool> filter)
        {
            return _calls.Where(x => filter(x.OutputType()));
        }
    }
}