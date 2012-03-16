﻿using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.View.Model;

namespace FubuMVC.Spark.SparkModel
{
    public interface ISharedTemplateLocator
    {
        ITemplate LocateMaster(string masterName, ITemplate fromTemplate);
        IEnumerable<ITemplate> LocateBindings(string bindingName, ITemplate fromTemplate); 
    }

    public class SharedTemplateLocator : ISharedTemplateLocator
    {
        private readonly ITemplateDirectoryProvider<ITemplate> _provider;
        private readonly ITemplateRegistry<ITemplate> _templates;

        public SharedTemplateLocator(ITemplateDirectoryProvider<ITemplate> provider, ITemplateRegistry<ITemplate> templates)
        {
            _provider = provider;
            _templates = templates;
        }

        public ITemplate LocateMaster(string masterName, ITemplate fromTemplate)
        {
            return locateTemplates(masterName, fromTemplate, true)
                .Where(x => x.IsSparkView())
                .FirstOrDefault();
        }

        public IEnumerable<ITemplate> LocateBindings(string bindingName, ITemplate fromTemplate)
        {
            return locateTemplates(bindingName, fromTemplate, false)
                .Where(x => x.IsXml());
        }

        private IEnumerable<ITemplate> locateTemplates(string name, ITemplate fromTemplate, bool sharedsOnly)
        {
            var directories = sharedsOnly 
                ? _provider.SharedPathsOf(fromTemplate) 
                : _provider.ReachablesOf(fromTemplate);

            return _templates.ByNameUnderDirectories(name, directories);
        }
    }
}