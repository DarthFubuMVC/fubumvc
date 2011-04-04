using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration
{
    public class HandlerActionsSet : IEnumerable<ActionCall>
    {
        private readonly Type _handlerType;
        private readonly IList<ActionCall> _calls = new List<ActionCall>();

        public HandlerActionsSet(IEnumerable<ActionCall> calls, Type handlerType)
        {
            _handlerType = handlerType;
            _calls.AddRange(calls.ToArray());
        }

        public Type HandlerType
        {
            get { return _handlerType; }
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

        public IEnumerator<ActionCall> GetEnumerator()
        {
            return _calls.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


    }
}