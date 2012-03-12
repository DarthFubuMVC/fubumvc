using System.IO;
using FubuCore;
using FubuCore.Util;
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

    public class ViewPathPolicy : ITemplatePolicy<ITemplate>
    {
        private readonly Cache<string, string> _cache;
        public ViewPathPolicy()
        {
            _cache = new Cache<string, string>(getPrefix);
        }

        public bool Matches(ITemplate template)
        {
            return template.ViewPath.IsEmpty();
        }

        public void Apply(ITemplate template)
        {
            template.ViewPath = FileSystem.Combine(_cache[template.Origin], template.RelativePath());
        }

        private static string getPrefix(string origin)
        {
            return origin == TemplateConstants.HostOrigin ? string.Empty : "_{0}".ToFormat(origin);
        }
    }
}