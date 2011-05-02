using System.Web.UI;

namespace FubuMVC.WebForms
{
    public interface INeedToKnowAboutParentPage
    {
        Page ParentPage { get; set; }
    }
}