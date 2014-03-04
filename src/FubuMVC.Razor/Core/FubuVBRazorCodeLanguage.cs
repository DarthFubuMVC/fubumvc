using System.Web.Razor;
using System.Web.Razor.Parser;

namespace FubuMVC.Razor.Core
{
    //from System.Web.Mvc
    public class FubuVBRazorCodeLanguage : VBRazorCodeLanguage
    {
        public override ParserBase CreateCodeParser()
        {
            return new FubuVBCodeParser();
        } 
    }
}