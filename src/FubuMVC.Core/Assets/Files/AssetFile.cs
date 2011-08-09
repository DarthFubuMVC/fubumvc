using System;

namespace FubuMVC.Core.Assets.Files
{
    /// <summary>
    ///  Make AssetFile dumb
    /// Forget CDN for now
    /// Forget LastChanged
    /// </summary>
    public class AssetFile 
    {
        public string Name { get; set; }
        public string FullPath { get; set; }
        //public DateTime LastChanged { get; set; }
        public bool Override { get; set; }

        public override string ToString()
        {
            var description = string.Format("Asset: {0} at {1}", Name, FullPath);
            if (Override)
            {
                description = description + " (Override)";
            }

            return description;
        }
    }
}