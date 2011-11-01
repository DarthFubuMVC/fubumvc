using System.Diagnostics;

namespace FubuMVC.Core.Assets.Diagnostics
{
    [DebuggerDisplay("{Provenance}: {Message}")]
    public class AssetLogEntry
    {
        public string Provenance { get; set; }
        public string Message { get; set; }
    }
}
