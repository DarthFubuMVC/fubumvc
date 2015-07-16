using System;
using System.Collections.Generic;

namespace FubuMVC.Core.Diagnostics.Packaging
{
    public interface IActivationLog
    {
        void Trace(ConsoleColor color, string text, params object[] parameters);
        void Trace(string text, params object[] parameters);

        void MarkFailure(Exception exception);
        void MarkFailure(string text, params object[] parameters);

        string FullTraceText();
        string Description { get; }
        bool Success { get; }
        void AddChild(params object[] child);
        IEnumerable<T> FindChildren<T>();
        void Execute(Action continuation);
        void TrapErrors(Action action);

    }
}