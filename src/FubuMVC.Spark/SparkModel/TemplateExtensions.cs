using System.IO;
using FubuCore;
using Spark;

namespace FubuMVC.Spark.SparkModel
{
    public static class TemplateExtensions
    {
        public static bool IsSparkView(this ISparkTemplate template)
		{
            return Path.GetExtension(template.FilePath).EqualsIgnoreCase(Constants.DotSpark) || Path.GetExtension(template.FilePath).EqualsIgnoreCase(Constants.DotShade);
        }

        public static bool IsXml(this ISparkTemplate template)
		{
            return Path.GetExtension(template.FilePath).EqualsIgnoreCase(".xml");
        }
    }
}