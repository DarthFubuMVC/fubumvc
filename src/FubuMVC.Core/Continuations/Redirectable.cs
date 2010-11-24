using System;
using System.Linq.Expressions;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Continuations
{
    public class Redirectable
    {
        public object Model { get; private set; }

        public Redirectable()
        {

        }

        public Redirectable(object model)
        {
            Model = model;
        }

        private FubuContinuation _continuation;
        public FubuContinuation Continuation
        {
            get
            {
                return _continuation ?? FubuContinuation.NextBehavior();
            }
            set
            {
                _continuation = value;
            }
        }

        public static Redirectable<T> FromModel<T>(T model) where T : class
        {
            return new Redirectable<T>(model);
        }
    }

    public class Redirectable<T> : Redirectable where T : class
    {
        public new T Model { get; private set; }

        public Redirectable()
        {

        }

        public Redirectable(T model)
        {
            Model = model;
        }

        public Redirectable(FubuContinuation continuation)
        {
            Continuation = continuation;
        }
    }


}
