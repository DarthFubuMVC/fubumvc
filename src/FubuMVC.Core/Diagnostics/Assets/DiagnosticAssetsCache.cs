using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Security.Permissions;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Diagnostics.Assets
{
    public interface IDiagnosticAssets
    {
        EmbeddedFile For(string name);
        void AddAssembly(Assembly assembly);
        IEnumerable<EmbeddedFile> JavascriptFiles();
    }

    [ReflectionPermission(SecurityAction.Assert)]
    public class DiagnosticAssetsCache : IDiagnosticAssets
    {
        private readonly IList<EmbeddedFile> _files = new List<EmbeddedFile>();
        private readonly Cache<string, EmbeddedFile> _searches;

        public DiagnosticAssetsCache(BehaviorGraph graph)
        {
            AddAssembly(Assembly.GetExecutingAssembly());
            graph.PackageAssemblies.Each(AddAssembly);

            _searches = new Cache<string, EmbeddedFile>(name =>
            {
                return _files.FirstOrDefault(x => x.Matches(name));
            });
        }

        public IEnumerable<EmbeddedFile> JavascriptFiles()
        {
            return _files.Where(x => x.ContentType == MimeType.Javascript);
        } 

        public EmbeddedFile For(string name)
        {
            return _searches[name];
        }

        public void AddAssembly(Assembly assembly)
        {
            var resources =
                assembly.GetManifestResourceNames()
                .Where(IsDiagnosticAsset)
                .Select(x => new EmbeddedFile(assembly, x));

            _files.AddRange(resources);
        }

        public static bool IsDiagnosticAsset(string path)
        {
            var parts = path.Split('.');
            return parts.Any(x => x.EqualsIgnoreCase("fubu_diagnostics"));
        }
    }
}