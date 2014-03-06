using System.IO;
using FubuCore;
using FubuMVC.Core.View.Model;

namespace FubuMVC.Spark.SparkModel
{
    public class NamespacePolicy : ITemplatePolicy<ISparkTemplate>
    {
        public bool Matches(ISparkTemplate template)
        {
            var descriptor = template.Descriptor as SparkDescriptor;
						
            return descriptor != null
				&& descriptor.Template.HasViewModel() 
				&& descriptor.Template.Namespace.IsEmpty();
        }

        public void Apply(ISparkTemplate template)
        {
            var relativePath = template.RelativePath();
            var relativeNamespace = Path.GetDirectoryName(relativePath);
            var descriptor = template.Descriptor.As<SparkDescriptor>();
            var nspace = descriptor.Template.ViewModel.Assembly.GetName().Name;
			
            if (relativeNamespace.IsNotEmpty())
            {
                nspace += "." + relativeNamespace.Replace(Path.DirectorySeparatorChar, '.');
            }
			
            descriptor.Template.Namespace = nspace;
        }
    }
}