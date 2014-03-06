using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.View.Model;

namespace FubuMVC.Spark.SparkModel
{
    public interface ISharedTemplateLocator : ISharedTemplateLocator<ISparkTemplate>
    {
        IEnumerable<ISparkTemplate> LocateBindings(string bindingName, ISparkTemplate fromTemplate); 
    }

    public class SharedTemplateLocator : SharedTemplateLocator<ISparkTemplate>, ISharedTemplateLocator
    {
        public SharedTemplateLocator(ITemplateDirectoryProvider<ISparkTemplate> provider, ITemplateRegistry<ISparkTemplate> templates)
            : base(provider, templates)
        {
        }

        public IEnumerable<ISparkTemplate> LocateBindings(string bindingName, ISparkTemplate fromTemplate)
        {
            throw new NotImplementedException();
//            return locateTemplates(bindingName, fromTemplate, false)
//                .Where(x => x.IsXml());
        }
    }
}