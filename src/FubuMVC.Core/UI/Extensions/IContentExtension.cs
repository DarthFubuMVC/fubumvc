using System.Collections.Generic;
using FubuMVC.Core.View;

namespace FubuMVC.Core.UI.Extensions
{
    public interface IContentExtension<T> where T : class
    {
        IEnumerable<object> GetExtensions(IFubuPage<T> page);
    }
}