using FubuMVC.Core;
using HtmlTags;

namespace AssemblyPackage
{
    public class AssemblyPackageRegistry : FubuPackageRegistry
    {
    }

    public class AssemblyEndpoint
    {
        public string get_hello()
        {
            return "Hello.";
        }
    }
}