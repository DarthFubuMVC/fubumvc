using System.IO;
using System.Net.Mime;
using System.Web;
using System.Web.UI;
using FubuMVC.Core.Runtime;

namespace FubuMVC.WebForms
{
    public class WebFormRenderer : IWebFormRenderer
    {
        private readonly IOutputWriter _outputWriter;

        public WebFormRenderer(IOutputWriter writer)
        {
            _outputWriter = writer;
        }

        public void RenderControl(Control control)
        {
            IHttpHandler handler = GetHandler(control);
            string html = ExecuteHandler(handler);
            _outputWriter.Write(MediaTypeNames.Text.Html, html);
        }


        public virtual IHttpHandler GetHandler(Control control)
        {
            var handler = control as IHttpHandler;

            if (handler == null)
            {
                var holderPage = new Page();
                holderPage.Controls.Add(control);
                handler = holderPage;
            }

            return handler;
        }

        public virtual string ExecuteHandler(IHttpHandler handler)
        {
            var writer = new StringWriter();
            HttpContext.Current.Server.Execute(handler, writer, true);
            writer.Flush();

            // See if this can just write out to the buffer
            return writer.GetStringBuilder().ToString();
        }
    }
}