using System.Collections;
using System.Collections.Generic;
using FubuMVC.Core.View;

namespace FubuMVC.UI.Extensibility
{
    public interface IContentExtension<T> where T : class
    {
        IEnumerable<object> GetExtensions(IFubuPage<T> page);
    }
}