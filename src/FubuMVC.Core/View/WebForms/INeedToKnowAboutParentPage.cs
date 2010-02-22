using System.Web.UI;

namespace FubuMVC.Core.View.WebForms
{
    public interface INeedToKnowAboutParentPage
    {
        Page ParentPage { get; set; }
    }
}