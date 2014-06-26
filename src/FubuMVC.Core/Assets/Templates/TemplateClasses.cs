using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using StructureMap.Util;

namespace FubuMVC.Core.Assets.Templates
{
    public class TemplateDef
    {
        public TemplateDef(BehaviorChain chain)
        {
        }

        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string File
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }

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
            throw new NotImplementedException();
        } 
    }
}