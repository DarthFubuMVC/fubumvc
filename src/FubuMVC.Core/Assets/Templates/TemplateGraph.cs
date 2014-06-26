using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Routing;
using FubuCore;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Conditionals;
using StructureMap.Util;

namespace FubuMVC.Core.Assets.Templates
{


    [Singleton]
    public class TemplateGraph
    {
        private readonly AssetSettings _settings;
        private readonly IServiceFactory _services;
        private readonly Lazy<LightweightCache<string, TemplateDef>> _templates;
        private readonly string _templateDirectory;
        private readonly IFileSystem _files = new FileSystem();

        public TemplateGraph(BehaviorGraph behaviors, AssetSettings settings, IServiceFactory services)
        {
            _settings = settings;
            _services = services;

            _templates = new Lazy<LightweightCache<string, TemplateDef>>(() => {
                return buildTemplateCache(behaviors);
            });

            _templateDirectory = FubuMvcPackageFacility
                .GetApplicationPath()
                .AppendPath(_settings.TemplateDestination)
                .ToFullPath()
                .Replace('\\', '/');
        }

        private LightweightCache<string, TemplateDef> buildTemplateCache(BehaviorGraph behaviors)
        {
            var cache = new LightweightCache<string, TemplateDef>();

            behaviors.Behaviors.Where(x => x.InputType().CanBeCastTo<Template>()).Each(chain => {
                var def = new TemplateDef(this, chain);

                cache[def.Name] = def;
            });

            return cache;
        }

        public string TemplateDirectory
        {
            get { return _templateDirectory; }
        }

        public IEnumerable<TemplateDef> Templates()
        {
            return _templates.Value;
        }

        public TemplateDef this[string nameOrFile]
        {
            get
            {
                return _templates.Value.Has(nameOrFile)
                    ? _templates.Value[nameOrFile]
                    : _templates.Value.FirstOrDefault(x => x.File == nameOrFile);
            }
        }

        public void WriteAll()
        {
            CleanAll();

            var tasks = _settings.TemplateCultures
                .Select(x => new CultureInfo(x))
                .Select(writeCulture)
                .ToArray();

            Task.WaitAll(tasks);
        }

        private Task writeCulture(CultureInfo culture)
        {
            return Task.Factory.StartNew(() => {
                SetCulture(culture);

                _templates.Value.Each(x => x.Write(culture));
            });
        }

        public void CleanAll()
        {
            _files.CleanDirectory(TemplateDirectory);
        }

        public void Write(string nameOrFile, string culture = "en-US")
        {
            var cultureInfo = new CultureInfo(culture);
            var template = this[nameOrFile];
            if (template == null)
            {
                throw new ArgumentOutOfRangeException("Unable to find a template named '{0}'".ToFormat(nameOrFile));
            }

            template.Write(cultureInfo);
        }

        public static void SetCulture(CultureInfo culture)
        {
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = culture;
        }

        public class TemplateDef
        {
            private readonly TemplateGraph _parent;
            private readonly BehaviorChain _chain;
            private readonly string _name;
            private readonly string _file;

            public TemplateDef(TemplateGraph parent, BehaviorChain chain)
            {
                _parent = parent;
                _chain = chain;

                var view = chain.Output.DefaultView();
                if (view == null)
                {
                    _name = chain.InputType().Name;
                }
                else
                {
                    _name = Path.GetFileNameWithoutExtension(view.Name());
                    _file = view.FilePath;
                }

            }

            public string PathFor(CultureInfo culture)
            {
                return _parent.TemplateDirectory.AppendPath(culture.Name, _name + ".htm").Replace('\\', '/');
            }

            public string Name
            {
                get { return _name; }
            }

            public string File
            {
                get { return _file; }
            }

            public void Write(CultureInfo culture)
            {                
                // Do this in a different way?
                SetCulture(culture);

                var text = GenerateTextForCurrentThreadCulture();

                var path = PathFor(culture);

                _parent._files.WriteStringToFile(path, text);
            }



            public string GenerateTextForCurrentThreadCulture()
            {
                var request = OwinHttpRequest.ForTesting();

                request.ContentType(MimeType.HttpFormMimetype);
                request.Accepts(MimeType.Html.Value);


                var services = new OwinServiceArguments(new RouteData(), request.Environment);
                var invoker = new BehaviorInvoker(_parent._services, _chain);
                invoker.Invoke(services);

                var response = new OwinHttpResponse(request.Environment);

                return response.Body.ReadAsText();
            }
        }
    }
}