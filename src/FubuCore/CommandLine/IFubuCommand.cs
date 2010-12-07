using System;

namespace FubuCore.CommandLine
{
    public interface IFubuCommand
    {
        void Execute(object input);
        Type InputType { get; }
    }

    // Later, think about a writer / logger

    //public class LinkPackageCommand
}