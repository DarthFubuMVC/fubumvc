using System.ComponentModel;

namespace Fubu
{
    public class PackagesInput
    {
        [Description("Physical root folder (or valid alias) of the application")]
        public string AppFolder { get; set; }
        
        [Description("Removes all 'exploded' package folders out of the application folder")]
        public bool CleanAllFlag { get; set; }

        [Description("'Explodes' all the zip files underneath <appfolder>/bin/fubu-packages")]
        public bool ExplodeFlag { get; set; }

        [Description("Removes all package zip files and exploded directories from the application folder")]
        public bool RemoveAllFlag { get; set; }
    }
}