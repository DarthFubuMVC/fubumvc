using System.Collections.Generic;

namespace FubuMVC.Core.Packaging
{
    public class AssemblyFiles
    {
        public IEnumerable<string> Files { get; set; }
        public IEnumerable<string> PdbFiles { get; set; }

        public IEnumerable<string> MissingAssemblies { get; set; }

        public bool Success { get; set; }
    }
}