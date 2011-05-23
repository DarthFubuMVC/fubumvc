using System.ComponentModel;

namespace Fubu
{
    public class ManifestInput
    {
        [Description("Physical folder (or valid alias) of the application")]
        public string AppFolder { get; set; }

        [Description("Opens the manifest file in notepad")]
        public bool OpenFlag { get; set; }
    }
}