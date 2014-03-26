using System.ComponentModel;

namespace FakeBottle
{
    public class FubuDiagnosticsConfiguration
    {
        public string Url = "fake";
        public string Title = "Fake";
        public string Description = "Some fake diagnostics";
    }

    public class FakeBottleFubuDiagnostics
    {
        [Description("Three")]
        public string get_three()
        {
            return "Three";
        }

        [Description("Four")]
        public string get_four()
        {
            return "Four";
        } 
    }
}