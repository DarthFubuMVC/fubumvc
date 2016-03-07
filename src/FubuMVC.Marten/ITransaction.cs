using System;
using FubuCore.Binding;

namespace FubuMVC.Marten
{
    public interface ITransaction
    {
        void Execute<T>(ServiceArguments arguments, Action<T> action) where T : class;

        void Execute<T>(Action<T> action) where T : class;

    }
}