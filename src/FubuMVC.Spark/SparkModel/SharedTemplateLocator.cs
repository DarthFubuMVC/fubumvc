using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.View.Model;

namespace FubuMVC.Spark.SparkModel
{
    public interface ISharedTemplateLocator : ISharedTemplateLocator<ITemplate>
    {
        IEnumerable<ITemplate> LocateBindings(string bindingName, ITemplate fromTemplate); 
    }

    public class SharedTemplateLocator : SharedTemplateLocator<ITemplate>, ISharedTemplateLocator
    {
        public SharedTemplateLocator(ITemplateDirectoryProvider<ITemplate> provider, ITemplateRegistry<ITemplate> templates, ITemplateSelector<ITemplate> templateSelector)
            : base(provider, templates, templateSelector)
        {
        }

        public IEnumerable<ITemplate> LocateBindings(string bindingName, ITemplate fromTemplate)
        {
            return locateTemplates(bindingName, fromTemplate, false)
                .Where(x => x.IsXml());
        }
    }
}