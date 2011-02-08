using System.Collections.Generic;
using Spark.FileSystem;
using Spark.Web.FubuMVC.ViewLocation;

namespace Spark.Web.FubuMVC
{
    public interface ISparkViewFactory
    {
        IViewFolder ViewFolder { get; set; }
        IDescriptorBuilder DescriptorBuilder { get; set; }
        ISparkSettings Settings { get; set; }
        ISparkViewEngine Engine { get; set; }
        ICacheServiceProvider CacheServiceProvider { get; set; }
        List<SparkViewDescriptor> CreateDescriptors(SparkBatchDescriptor batch, string viewLocatorName);
        IList<SparkViewDescriptor> CreateDescriptors(SparkBatchEntry entry, string viewLocatorName);
        SparkViewDescriptor CreateDescriptor(string targetNamespace, string actionName, string viewName, string masterName, bool findDefaultMaster);
    	ViewEngineResult FindView(ActionContext actionContext, string viewName, string masterName);
    	ViewEngineResult FindPartialView(ActionContext actionContext, string partialViewName);
    }
}