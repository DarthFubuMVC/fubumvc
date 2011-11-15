using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Assets;
using FubuMVC.Core.UI;

namespace Serenity.Jasmine
{
    public class SpecAssetRequirements
    {
        private readonly IAssetRequirements _requirements;

        public SpecAssetRequirements(IAssetRequirements requirements)
        {
            _requirements = requirements;
        }

        public void WriteAssetsInto(FubuHtmlDocument document, IEnumerable<Specification> specs)
        {
            RegisterRequirements(specs);
            document.WriteAssetsToHead();

            RegisterSpecifications(specs);
            document.WriteAssetsToHead();
        }

        public void RegisterRequirements(IEnumerable<Specification> specs)
        {
            _requirements.Require("core");
            _requirements.Require("jasmine");

            specs.SelectMany(x => x.Libraries).Each(lib => _requirements.Require(lib.Name));
        }

        public void RegisterSpecifications(IEnumerable<Specification> specs)
        {
            specs.Each(spec => _requirements.Require(spec.File.Name));
        }

        public void WriteBasicAssetsInto(FubuHtmlDocument document)
        {
            _requirements.Require("jquery", "jquery.treeview.js", "jasmine.treeview.css");
            document.WriteAssetsToHead();
        }
    }
}