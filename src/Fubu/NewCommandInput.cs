using FubuCore.CommandLine;

namespace Fubu
{
    public class NewCommandInput
    {
        public string ProjectName { get; set; }

        [FlagAlias("z")]
        public string ZipFlag { get; set; }
    }
}