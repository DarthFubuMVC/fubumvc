using System.Web;
using FubuMVC.Razor.Rendering;

namespace FubuMVC.Razor
{
    public static class RazorViewExtensions
    {
         public static HtmlString RenderPartial(this IFubuRazorView view, string name)
         {
             return view.Get<IPartialRenderer>().Render(view, name);
         }

         public static HtmlString RenderPartial(this IFubuRazorView view, string name, object model)
         {
             return view.Get<IPartialRenderer>().Render(view, name, model);
         }
    }
}