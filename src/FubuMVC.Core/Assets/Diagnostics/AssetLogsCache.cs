using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuCore;

namespace FubuMVC.Core.Assets.Diagnostics
{
    [DebuggerDisplay("Asset Logs ( {Entries.Count} Entries)")]
    public class AssetLogsCache
    {
        public AssetLogsCache()
        {
            Entries = new List<AssetLog>();
        }

        public IList<AssetLog> Entries { get; set; }


        public AssetLog FindByName(string name)
        {
            var result = Entries.FirstOrDefault(e => e.Name.EqualsIgnoreCase(name));
            if (result == null)
            {
                result = new AssetLog(name);
                Entries.Add(result);
            }
            return result;
        }

    }
}