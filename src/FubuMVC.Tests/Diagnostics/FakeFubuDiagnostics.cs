namespace FubuMVC.Tests.Diagnostics
{
    public class FakeFubuDiagnostics
    {
        public string Index()
        {
            return null;
        }

        [System.ComponentModel.Description("A Title:A really good display of some important data")]
        public string get_link()
        {
            return "is a link";
        }

        [System.ComponentModel.Description("Simple Title")]
        public string get_simple()
        {
            return null;
        }

        public string get_else()
        {
            return null;
        }

        public string get_query_Name(FakeQuery query)
        {
            return null;
        }

        public string post_ask()
        {
            return null;
        }
    }
}