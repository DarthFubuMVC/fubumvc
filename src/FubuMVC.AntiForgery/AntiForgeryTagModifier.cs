using FubuMVC.Core.UI;
using HtmlTags;
using FubuMVC.Core.UI.Forms;
using HtmlTags.Conventions;

namespace FubuMVC.AntiForgery
{
    public class AntiForgeryTagModifier : ITagModifier<FormRequest>
    {
        public bool Matches(FormRequest token)
        {
            return true;
        }

        public void Modify(FormRequest request)
        {
            var antiForgeryService = request.Services.GetInstance<IAntiForgeryService>();
            var antiForgerySettings = request.Services.GetInstance<AntiForgerySettings>();
            AntiForgeryData cookieToken = antiForgeryService.SetCookieToken(antiForgerySettings.Path, antiForgerySettings.Domain);
            var salt = request.Chain.InputType().FullName;
            FormToken formToken = antiForgeryService.GetFormToken(cookieToken, salt);
            var antiForgeryTag = new HiddenTag().Name(formToken.Name).Value(formToken.TokenString);
            request.CurrentTag.Append(antiForgeryTag);
        }
    }
}