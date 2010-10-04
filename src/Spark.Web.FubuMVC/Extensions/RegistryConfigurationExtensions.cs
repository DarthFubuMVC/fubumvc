using System;
using FubuMVC.Core.Registration.DSL;
using Spark.Web.FubuMVC.Bootstrap;
using FubuMVC.Core.Registration.Nodes;

namespace Spark.Web.FubuMVC.Extensions
{
    public static class RegistryConfigurationExtensions
    {
        public static void BySparkViewDescriptors(this ViewsForActionFilterExpression expression, 
            Func<Type, string> getViewLocatorNameFromCallConvention, 
            Func<ActionCall, string> getViewNameFromActionCallConvention)
        {
            expression.by(new ActionAndViewMatchedBySparkViewDescriptors(getViewLocatorNameFromCallConvention, getViewNameFromActionCallConvention));
        }
    }
}