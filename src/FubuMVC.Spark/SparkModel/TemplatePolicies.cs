using System.IO;
using FubuCore;
using FubuMVC.Core.View.Model;

namespace FubuMVC.Spark.SparkModel
{
    public class NamespacePolicy : ITemplatePolicy<ITemplate>
    {
        public bool Matches(ITemplate template)
        {
            var descriptor = template.Descriptor as SparkDescriptor;
						
            return descriptor != null
				&& descriptor.HasViewModel() 
				&& descriptor.Namespace.IsEmpty();
        }

        public void Apply(ITemplate template)
        {
            var relativePath = template.RelativePath();
            var relativeNamespace = Path.GetDirectoryName(relativePath);
            var descriptor = template.Descriptor.As<SparkDescriptor>();
            var nspace = descriptor.ViewModel.Assembly.GetName().Name;
			
            if (relativeNamespace.IsNotEmpty())
            {
                nspace += "." + relativeNamespace.Replace(Path.DirectorySeparatorChar, '.');
            }
			
            descriptor.Namespace = nspace;
        }
    }
}