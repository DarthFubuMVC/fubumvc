using System;

namespace FubuCore
{
    public interface IScoped<T>
    {
        void Do(Action<T> action);
        TResult Get<TResult>(Func<T, TResult> func);
    }

    public class SimpleScoped<T> : IScoped<T>
    {
        private readonly T _target;

        public SimpleScoped(T target)
        {
            _target = target;
        }

        public void Do(Action<T> action)
        {
            action(_target);
        }

        public TResult Get<TResult>(Func<T, TResult> func)
        {
            return func(_target);
        }
    }



}