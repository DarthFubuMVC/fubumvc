using FubuMVC.Core.UI.Forms;

namespace FubuMVC.Core.Validation.Web.UI
{
    public interface IRenderingStrategy
    {
        void Modify(FormRequest request);
    }
}