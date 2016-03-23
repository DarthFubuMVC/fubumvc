using HtmlTags;

namespace FubuMVC.Core.Security.Authentication.Saml2
{
    public interface ISamlResponseRedirector
    {
        HtmlDocument WriteRedirectionHtml(SamlResponse response);
    }
}