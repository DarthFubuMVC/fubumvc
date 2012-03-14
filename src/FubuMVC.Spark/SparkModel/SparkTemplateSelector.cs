using FubuMVC.Core.View.Model;

namespace FubuMVC.Spark.SparkModel
{
    public class SparkTemplateSelector : ITemplateSelector<ITemplate>
    {
        public bool IsAppropriate(ITemplate template)
        {
            return template.IsSparkView();
        }
    }
}