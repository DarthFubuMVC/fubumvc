using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace FubuMVC.Core.View.Model
{
    public interface ISharedTemplateLocator<T> where T : ITemplateFile
    {
        T LocateMaster(string masterName, T fromTemplate);
        T LocatePartial(string partialName, T fromTemplate);
    }

    // TODO -- rewrite this puppy
    public class SharedTemplateLocator<T> : ISharedTemplateLocator<T> where T : ITemplateFile
    {
        public T LocateMaster(string masterName, T fromTemplate)
        {
            throw new System.NotImplementedException();
        }

        public T LocatePartial(string partialName, T fromTemplate)
        {
            throw new System.NotImplementedException();
        }
    }

}