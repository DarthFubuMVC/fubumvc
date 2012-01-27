using System;
using System.IO;
using System.Web;
using System.Web.Compilation;
using System.Web.UI;

namespace FubuMVC.WebForms
{
    public interface IWebFormsControlBuilder
    {
        Control LoadControlFromVirtualPath(string virtualPath, Type mustImplementInterfaceType);
        void ExecuteControl(IHttpHandler handler, TextWriter writer);
    }

    // THAR BE DRAGONS!
    // Untestable ASP.NET code to follow....
    public class WebFormsControlBuilder : IWebFormsControlBuilder
    {
        private readonly HttpContextBase _httpContext;

        public WebFormsControlBuilder(HttpContextBase httpContext)
        {
            _httpContext = httpContext;
        }

        public virtual Control LoadControlFromVirtualPath(string virtualPath, Type mustImplementInterfaceType)
        {
            return (Control) BuildManager.CreateInstanceFromVirtualPath(virtualPath, mustImplementInterfaceType);
        }

        public void ExecuteControl(IHttpHandler handler, TextWriter writer)
        {
            _httpContext.Server.Execute(handler, writer, true);
        }
    }
}