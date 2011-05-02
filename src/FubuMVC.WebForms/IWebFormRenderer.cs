using System.Web.UI;

namespace FubuMVC.WebForms
{
    public interface IWebFormRenderer
    {
        void RenderControl(Control control);
    }
}