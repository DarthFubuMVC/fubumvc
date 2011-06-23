using System.ComponentModel;
using FubuCore.CommandLine;

namespace Fubu
{
    public class NewCommandInput
    {
        [FlagAlias("z")]
        [Description("A zip file containing a FubuMVC project template")]
        public string ZipFlag { get; set; }

        [FlagAlias("g")]
        [Description("A git repository containing a FubuMVC project template")]
        public string GitFlag { get; set; }

        [Description("The name and destination folder of the new FubuMVC project")]
        public string ProjectName { get; set; }
    }
}