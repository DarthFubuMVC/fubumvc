using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Http;
using FubuCore;

namespace FubuMVC.Core.Assets
{
    public interface IAssetUrls
    {
        /// <summary>
        /// Resolve the url for a named asset
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        string UrlForAsset(AssetFolder folder, string name);
    }

    public class AssetUrls : IAssetUrls
    {
        private readonly ICurrentHttpRequest _httpRequest;
        public static readonly string AssetsUrlFolder = "_content";

        public AssetUrls(ICurrentHttpRequest httpRequest)
        {
            _httpRequest = httpRequest;
        }

        /// <summary>
        /// Resolve the url for a named asset
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public string UrlForAsset(AssetFolder folder, string name)
        {
            var relativeUrl = DetermineRelativeAssetUrl(folder, name);
            return _httpRequest.ToFullUrl(relativeUrl);
        }

        // TODO -- move the unit tests
        public static string DetermineRelativeAssetUrl(IAssetTagSubject subject)
        {
            var folder = subject.Folder;
            var name = subject.Name;

            return DetermineRelativeAssetUrl(folder, name);
        }

        // TODO -- move the unit tests
        public static string DetermineRelativeAssetUrl(AssetFolder folder, string name)
        {
            return "{0}/{1}/{2}".ToFormat(AssetsUrlFolder, folder, name);
        }
    }
}