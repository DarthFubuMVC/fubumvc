using System;
using System.Collections.Generic;
using FubuCore.Util;
using FubuMVC.Core.Assets.Files;
using System.Linq;
using FubuCore;

namespace FubuMVC.Core.Assets.Content
{
    public class TransformerComparer : IComparer<ITransformerPolicy>
    {
        private readonly AssetFile _file;
        private readonly Cache<ITransformerPolicy, int?> _positions;
        private readonly Lazy<IList<string>> _extensions;

        public TransformerComparer(AssetFile file)
        {
            _file = file;
            _extensions = new Lazy<IList<string>>(() => _file.AllExtensions().ToList());

            _positions = new Cache<ITransformerPolicy, int?>(policy => policy.MatchingExtensionPosition(_extensions.Value));
        }

        public int Compare(ITransformerPolicy x, ITransformerPolicy y)
        {
            if (ReferenceEquals(x, y)) return 0;

            // if x is > y, return 1
            // if y is > x, return -1
            // no diff, return 0

            if (x.ActionType == y.ActionType)
            {
                var xPos = x.MatchingExtensionPosition(_extensions.Value);                
                var yPos = y.MatchingExtensionPosition(_extensions.Value);                
            
                if (xPos.HasValue && yPos.HasValue)
                {
                    return xPos.Value.CompareTo(yPos.Value);
                }
            }
            else
            {
                return x.ActionType.As<int>().CompareTo(y.ActionType.As<int>());
            }

            if (x.MustBeAfter(y)) return 1;
            if (y.MustBeAfter(x)) return -1;

            return 0;
        }
    }
}