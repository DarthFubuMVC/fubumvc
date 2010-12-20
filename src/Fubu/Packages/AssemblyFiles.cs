using System.Collections.Generic;

namespace Fubu.Packages
{
    public class AssemblyFiles
    {
        public IEnumerable<string> Files { get; set; }
        public IEnumerable<string> PdbFiles { get; set; }

        public IEnumerable<string> MissingAssemblies { get; set; }

        public bool Success { get; set; }
    }
}