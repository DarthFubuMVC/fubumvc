using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Reflection;
using System.Security.Permissions;
using System.Web.Caching;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Diagnostics.Assets
{
    [ReflectionPermission(SecurityAction.Assert)]
    public class DiagnosticAssets
    {
        private readonly IList<EmbeddedFile> _files = new List<EmbeddedFile>();
        private readonly Cache<string, EmbeddedFile> _searches;

        public DiagnosticAssets()
        {
            AddAssembly(Assembly.GetExecutingAssembly());

            _searches = new Cache<string, EmbeddedFile>(name =>
            {
                return _files.FirstOrDefault(x => x.Matches(name));
            });
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
            return parts.Any(x => x.EqualsIgnoreCase("diagnostics"));
        }
    }

    public class EmbeddedFile
    {
        private readonly Lazy<byte[]> _contents;
        private readonly string _cacheHeader = "private, max-age={0}".ToFormat(AssetSettings.MaxAgeInSeconds);
        

        public EmbeddedFile(Assembly assembly, string resource)
        {
            Name = resource;
            ContentType = MimeType.MimeTypeByFileName(Name);


            _contents = new Lazy<byte[]>(() =>
            {
                var stream = assembly.GetManifestResourceStream(resource);
                return stream.ReadAllBytes();
            });

            Version = assembly.GetName().Version.ToString();
        }

        public bool Matches(string file)
        {
            return Name.EndsWith(file, StringComparison.OrdinalIgnoreCase);
        }

        // Only hitting this with integration tests
        public void Write(IHttpResponse response)
        {
            response.WriteContentType(ContentType.Value);
            response.WriteResponseCode(HttpStatusCode.OK);

            if (!FubuMode.InDevelopment())
            {
                response.AppendHeader(HttpResponseHeaders.CacheControl, _cacheHeader);
                var expiresKey = DateTime.UtcNow.AddSeconds(AssetSettings.MaxAgeInSeconds).ToString("R");
                response.AppendHeader(HttpResponseHeaders.Expires, expiresKey);
            }
        }


        public MimeType ContentType { get; private set; }

        public string Name { get; private set; }
        public string Version { get; private set; }

        public byte[] Contents()
        {
            return _contents.Value;
        }
    }
}