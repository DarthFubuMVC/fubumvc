using FubuMVC.Core.Assets;
using FubuMVC.Core.View;
using HtmlTags;

namespace FubuMVC.IntegrationTesting.Serenity.App
{
    public class AppErrorEndpoint
    {
        private readonly FubuHtmlDocument _document;

        public AppErrorEndpoint(FubuHtmlDocument document)
        {
            _document = document;
        }

        public HtmlDocument get_javascript_error()
        {
            _document.Title = "Javascript Error";
            _document.Body.Append(_document.Script("errors.js"));

            return _document;
        } 
    }
}