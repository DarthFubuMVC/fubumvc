using FubuMVC.Core.View.Model;

namespace FubuMVC.Spark.SparkModel
{
    public class SparkTemplateSelector : ITemplateSelector<ISparkTemplate>
    {
        public bool IsAppropriate(ISparkTemplate template)
        {
            return template.IsSparkView();
        }
    }
}