using System.Collections.Generic;

namespace FubuMVC.Core.Diagnostics.Packaging
{
    public class PackageDiagnosticsModel
    {
        public PackageDiagnosticsModel()
        {
            logs = new List<PackageDiagnosticsLogModel>();
        }

        public IEnumerable<PackageDiagnosticsLogModel> logs { get; set; }
    }
}