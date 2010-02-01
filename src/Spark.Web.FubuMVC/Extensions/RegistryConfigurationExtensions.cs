using System;
using FubuMVC.Core.Registration.DSL;
using Spark.Web.FubuMVC.Bootstrap;

namespace Spark.Web.FubuMVC.Extensions
{
    public static class RegistryConfigurationExtensions
    {
        public static void BySparkViewDescriptors(this ViewsForActionFilterExpression expression, Func<string, string> getActionNameFromCallConvention)
        {
            expression.by(new ActionAndViewMatchedBySparkViewDescriptors(getActionNameFromCallConvention));
        }
    }
}