using System;
using Spark;
using System.IO;

namespace Spark.Web.FubuMVC.ViewCreation
{
    public class ViewContext : ActionContext
    {
        public ViewContext(ActionContext actionContext, ISparkView view, TextWriter writer) :
            base(actionContext.HttpContext, actionContext.RouteData, actionContext.ActionNamespace)
        {
            Writer = writer;
            View = view;
        }

        public ISparkView View { get; set; }
        public virtual TextWriter Writer { get; set; }
    }
}
