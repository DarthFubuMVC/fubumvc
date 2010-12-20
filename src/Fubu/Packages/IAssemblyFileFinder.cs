using System.Collections.Generic;

namespace Fubu.Packages
{
    public interface IAssemblyFileFinder
    {
        AssemblyFiles FindAssemblies(string binDirectory, IEnumerable<string> assemblyNames);
    }
}