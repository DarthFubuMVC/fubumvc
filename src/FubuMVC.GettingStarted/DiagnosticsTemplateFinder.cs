using System.Linq;
using FubuCore;
using FubuMVC.Spark.SparkModel;

namespace FubuMVC.GettingStarted
{
    public class DiagnosticsTemplateBinder : ITemplateBinder
    {
        public bool CanBind(IBindRequest request)
        {
            var descriptor = request.Target.Descriptor as ViewDescriptor;
            if(descriptor == null || descriptor.ViewModel == null)
            {
                return false;
            }

            return descriptor.ViewModel.Assembly.Equals(GetType().Assembly);
        }

        public void Bind(IBindRequest request)
        {
            var master = request
                .TemplateRegistry
                .ByOrigin("FubuMVC.Diagnostics")
                .FirstOrDefault(t => t.Name().ToLower().Equals("application"));
            if(master == null)
            {
                return;
            }

            request
                .Target
                .Descriptor
                .As<ViewDescriptor>()
                .Master = master;

            request
                .Logger
                .Log(request.Target, "Master page [{0}] found at {1}", master.Name(), master.FilePath);
        }
    }
}