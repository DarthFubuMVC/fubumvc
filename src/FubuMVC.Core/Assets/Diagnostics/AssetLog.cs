using System.Collections.Generic;

namespace FubuMVC.Core.Assets.Diagnostics
{
    public class AssetLog
    {
        public AssetLog(string name)
        {
            Name = name;
            Logs = new List<AssetLogEntry>();
        }

        public string Name { get; set; }
        public IList<AssetLogEntry> Logs { get; set; }

        public void Add(string provenance, string message)
        {
            Logs.Add(new AssetLogEntry(){Provenance = provenance, Message =  message});
        }
    }
}