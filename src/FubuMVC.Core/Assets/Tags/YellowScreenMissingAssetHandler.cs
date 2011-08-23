using System.Collections.Generic;
using System.Linq;
using HtmlTags;

namespace FubuMVC.Core.Assets.Tags
{
    public class YellowScreenMissingAssetHandler : IMissingAssetHandler
    {
        public IEnumerable<HtmlTag> BuildTagsAndRecord(IEnumerable<MissingAssetTagSubject> subjects)
        {
            if (subjects.Any())
            {
                throw new MissingAssetsException(subjects);
                
            }

            return new HtmlTag[0];
        }
    }
}