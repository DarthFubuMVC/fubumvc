using System.Collections.Generic;

namespace FubuMVC.Diagnostics.Models
{
    public class PackageDiagnosticsModel
    {
        public PackageDiagnosticsModel()
        {
            Logs = new List<PackageDiagnosticsLogModel>();
        }

        public IEnumerable<PackageDiagnosticsLogModel> Logs { get; set; }
    }
}