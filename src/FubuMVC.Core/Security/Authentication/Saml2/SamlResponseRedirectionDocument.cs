using HtmlTags;

namespace FubuMVC.Core.Security.Authentication.Saml2
{
    public class SamlResponseRedirectionDocument : HtmlDocument
    {
        public SamlResponseRedirectionDocument(string response, string destination)
        {
            Title = "Saml2 Response Redirection";

            var form = new FormTag(destination);

            Push(form);

            var hiddenTag = new HiddenTag().Attr("name", SamlAuthenticationStrategy.SamlResponseKey)
                                           .Attr("value", response);

            Add(hiddenTag);

            Pop();

            AddJavaScript("window.onload = function () { document.forms[0].submit(); }");
        }
    }
}