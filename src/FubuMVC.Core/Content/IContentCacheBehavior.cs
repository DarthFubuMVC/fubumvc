using System;
using System.Web;

namespace FubuMVC.Core.Content
{
    public interface IContentCacheBehavior
    {
        void RegisterCacheBehavior(Func<string, Action<HttpCachePolicy>> cachingAction);
        void ApplyCacheBehavior(string filename, HttpCachePolicy cache);
    }
}