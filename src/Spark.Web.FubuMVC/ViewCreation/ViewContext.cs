using System;
using Spark;
using System.IO;

namespace Spark.Web.FubuMVC.ViewCreation
{
    public class ViewContext : ActionContext
    {
        public ViewContext(ActionContext actionContext, ISparkView view) : 
            base(actionContext.HttpContext, actionContext.RouteData, actionContext.ActionNamespace, actionContext.ActionName)
        {
            View = view;
        }

        public ISparkView View { get; set; }
    }
}
