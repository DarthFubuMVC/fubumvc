using System.IO;
using FubuCore;
using Spark;

namespace FubuMVC.Spark.SparkModel
{
    public static class TemplateExtensions
    {
        public static bool IsSparkView(this ITemplate template)
		{
            return Path.GetExtension(template.FilePath).EqualsIgnoreCase(Constants.DotSpark);
        }

        public static bool IsXml(this ITemplate template)
		{
            return Path.GetExtension(template.FilePath).EqualsIgnoreCase(".xml");
        }
    }
}