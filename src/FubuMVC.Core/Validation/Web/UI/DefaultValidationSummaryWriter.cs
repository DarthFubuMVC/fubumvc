using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using HtmlTags;

namespace FubuMVC.Core.Validation.Web.UI
{
    [Description("Writes the default html partial for the Validation Summary")]
    public class DefaultValidationSummaryWriter : IMediaWriter<ValidationSummary>
    {
        public virtual HtmlTag BuildSummary()
        {
            return new HtmlTag("div")
                .AddClasses("alert", "alert-error", "validation-container")
                .Append(new HtmlTag("p").Text(ValidationKeys.Summary))
                .Append(new HtmlTag("ul").AddClass("validation-summary"))
                .Style("display", "none");
        }

        public Task Write(string mimeType, IFubuRequestContext context, ValidationSummary resource)
        {
            return context.Writer.WriteHtml(BuildSummary());
        }

        public IEnumerable<string> Mimetypes
        {
            get
            {
                yield return MimeType.Html.Value;
                yield return MimeType.HttpFormMimetype;
            }
        }
    }
}