using System;
using System.Collections.Generic;
using FubuMVC.Core.View;

namespace FubuMVC.Core.UI.Extensions
{
    public class LambdaExtension<T> : IContentExtension<T> where T : class
    {
        private readonly Func<IFubuPage<T>, object> _func;

        public LambdaExtension(Func<IFubuPage<T>, object> func)
        {
            _func = func;
        }

        public IEnumerable<object> GetExtensions(IFubuPage<T> page)
        {
            yield return _func(page);
        }
    }
}