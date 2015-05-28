using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.UI;
using HtmlTags;

namespace FubuMVC.Authentication.Endpoints
{
    public class DefaultLoginRequestWriter : IMediaWriter<LoginRequest>
    {

        public void Write(string mimeType, IFubuRequestContext context, LoginRequest resource)
        {
            var document = BuildView(context, resource);
            context.Writer.WriteHtml(document.ToString());
        }

        public virtual HtmlDocument BuildView(IFubuRequestContext context, LoginRequest request)
        {
            // TODO -- Revisit all of this when we get HTML conventions everywhere
            var view = new FubuHtmlDocument<LoginRequest>(context.Services, context.Models);
            var form = view.FormFor<LoginRequest>();
            form.Append(new HtmlTag("legend").Text(LoginKeys.Login));

            if(request.Message.IsNotEmpty())
            {
                form.Append(new HtmlTag("p").Text(request.Message).Style("color", "red"));
            }

            form.Append(view.Edit(x => x.UserName));
            form.Append(view.Edit(x => x.Password));
            form.Append(view.Edit(x => x.RememberMe));
            form.Append(view.DisplayFor(x => x.Message).Id("login-message"));

            form.Append(new HiddenTag().Name("Url").Attr("value", request.Url));

            form.Append(new HtmlTag("input").Attr("type", "submit").Attr("value", LoginKeys.Login).Id("login-submit"));
            
            view.Add(form);

            return view;
        }

        public IEnumerable<string> Mimetypes
        {
            get { yield return MimeType.Html.Value; }
        }
    }
}