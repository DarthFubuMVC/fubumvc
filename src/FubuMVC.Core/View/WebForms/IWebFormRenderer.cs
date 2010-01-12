using System.Web.UI;

namespace FubuMVC.Core.View.WebForms
{
    public interface IWebFormRenderer
    {
        void RenderControl(Control control);
    }
}