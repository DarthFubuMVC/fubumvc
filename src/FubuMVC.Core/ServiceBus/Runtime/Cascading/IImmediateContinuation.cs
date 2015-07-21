using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Core.ServiceBus.Runtime.Cascading
{
    public interface IImmediateContinuation
    {
        object[] Actions();
    }

    public class ImmediateContinuation : IImmediateContinuation
    {
        private readonly IList<object> _actions = new List<object>();

        public ImmediateContinuation(params object[] actions)
        {
            _actions.AddRange(actions);
        }

        public object[] Actions()
        {
            return _actions.ToArray();
        }

        public ImmediateContinuation With(params object[] actions)
        {
            _actions.AddRange(actions);

            return this;
        }
    }

    public static class ContinueImmediately
    {
        public static ImmediateContinuation With(params object[] actions)
        {
            return new ImmediateContinuation(actions);
        }
    }
}