using System;
using StructureMap;

namespace FubuFastPack.StructureMap
{
    public interface ITransactionProcessor
    {
        void Execute<T>(Action<T> action);
        TReturn Execute<T, TReturn>(Func<T, TReturn> func);
        void Execute<T>(Action<T, IContainer> action);
        void Execute<T>(string instanceName, Action<T> action);
    }
}