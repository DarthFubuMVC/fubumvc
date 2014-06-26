using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Conditionals;
using StructureMap.Util;

namespace FubuMVC.Core.Assets.Templates
{
    public class TemplateDef
    {
        private readonly BehaviorChain _chain;
        private readonly string _name;
        private readonly string _file;

        public TemplateDef(BehaviorChain chain)
        {
            _chain = chain;

            var view = chain.Output.DefaultView();
            if (view == null)
            {
                throw new NotImplementedException();
            }
            else
            {
                _name = Path.GetFileNameWithoutExtension(view.Name());
                _file = view.FilePath;
            }

        }

        public string Name
        {
            get { return _name; }
        }

        public string File
        {
            get { return _file; }
        }
    }

    [Singleton]
    public class TemplateGraph
    {
        private readonly AssetSettings _settings;
        private readonly IServiceFactory _services;
        private readonly Lazy<LightweightCache<string, TemplateDef>> _templates; 

        public TemplateGraph(BehaviorGraph behaviors, AssetSettings settings, IServiceFactory services)
        {
            _settings = settings;
            _services = services;

            _templates = new Lazy<LightweightCache<string, TemplateDef>>(() => {
                return buildTemplateCache(behaviors);
            });
        }

        private static LightweightCache<string, TemplateDef> buildTemplateCache(BehaviorGraph behaviors)
        {
            var cache = new LightweightCache<string, TemplateDef>();

            behaviors.Behaviors.Where(x => x.InputType().CanBeCastTo<Template>()).Each(chain => {
                var def = new TemplateDef(chain);

                cache[def.Name] = def;
            });

            return cache;
        }

        public IEnumerable<TemplateDef> Templates()
        {
            return _templates.Value;
        } 
    }
}