using FubuMVC.Core.View.Model;

namespace FubuMVC.Spark.SparkModel
{
    public class ViewDescriptorBinder : ITemplateBinder<ITemplate>
    {
        public bool CanBind(IBindRequest<ITemplate> request)
        {
            var template = request.Target;
            return !(template.Descriptor is SparkDescriptor) && template.IsSparkView();
        }

        public void Bind(IBindRequest<ITemplate> request)
        {
            request.Target.Descriptor = new SparkDescriptor(request.Target);
        }
    }
}