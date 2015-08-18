using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security.Authentication.Endpoints;
using FubuMVC.Core.Urls;
using HtmlTags;

namespace FubuMVC.Core.Security.Authentication
{
    public class DefaultLoginRequestWriter : IMediaWriter<LoginRequest>
    {
        public virtual HtmlDocument BuildView(IUrlRegistry urls, IOutputWriter writer, LoginRequest request)
        {
            // TODO -- Revisit all of this when we get HTML conventions everywhere
            var view = new HtmlDocument();
            var form = new FormTag(urls.UrlFor<LoginRequest>("POST"));
            form.Append(new HtmlTag("legend").Text(LoginKeys.Login));

            if (request.Message.IsNotEmpty())
            {
                form.Append(new HtmlTag("p").Text(request.Message).Style("color", "red"));
            }

            form.Append(new TextboxTag("UserName", request.UserName));
            form.Append(new TextboxTag("Password", request.Password));
            form.Append(new CheckboxTag(request.RememberMe).Name("RememberMe"));


            form.Append(new DivTag().Text(request.Message).Id("login-message"));
            


            form.Append(new HiddenTag().Name("Url").Attr("value", request.Url));

            form.Append(new HtmlTag("input").Attr("type", "submit").Attr("value", LoginKeys.Login).Id("login-submit"));

            view.Add(form);

            return view;
        }

        public void Write(string mimeType, IFubuRequestContext context, LoginRequest resource)
        {
            context.Writer.Write(mimeType, BuildView(context.Service<IUrlRegistry>(), context.Writer, resource).ToString());
        }

        public IEnumerable<string> Mimetypes
        {
            get { yield return MimeType.Html.Value; }
        }
    }

}