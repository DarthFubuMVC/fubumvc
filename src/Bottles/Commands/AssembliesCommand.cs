using System;
using FubuCore;
using FubuCore.CommandLine;

namespace Bottles.Commands
{
    public enum AssembliesCommandMode
    {
        add,
        remove,
        list
    }

    public class AssembliesInput
    {
        public string Directory { get; set; }
        public string File { get; set; }
        public AssembliesCommandMode Mode { get; set; }
        public bool AllFlag { get; set; }
        public string AssemblyFlag { get; set; }
        public bool OpenFlag { get; set; }
    }

    public class AssembliesCommand : FubuCommand<AssembliesInput>
    {
        public override bool Execute(AssembliesInput input)
        {
            input.Directory = AliasCommand.AliasFolder(input.Directory);

            Execute(new FileSystem(), input);

            return true;
        }

        private void Execute(IFileSystem fileSystem, AssembliesInput input)
        {
            throw new NotImplementedException();
        }
    }
}