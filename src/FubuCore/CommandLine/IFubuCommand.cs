using System;

namespace FubuCore.CommandLine
{
    public interface IFubuCommand
    {
        void Execute(object input);
        Type InputType { get; }
    }

    public interface IFubuCommand<T> : IFubuCommand
    {
        
    }
}