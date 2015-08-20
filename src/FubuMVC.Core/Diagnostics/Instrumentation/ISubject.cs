using System;

namespace FubuMVC.Core.Diagnostics.Instrumentation
{
    public interface ISubject
    {
        string Title();
        Guid Id { get; }
    }
}