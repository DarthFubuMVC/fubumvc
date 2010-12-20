using System.Collections.Generic;

namespace FubuMVC.Core.Packaging
{
    public interface IAssemblyFileFinder
    {
        AssemblyFiles FindAssemblies(string binDirectory, IEnumerable<string> assemblyNames);
    }
}