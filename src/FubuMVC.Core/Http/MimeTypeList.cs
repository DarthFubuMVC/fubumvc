using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace FubuMVC.Core.Http
{
    public class MimeTypeList : IEnumerable<string>
    {
        private readonly IList<string> _mimeTypes = new List<string>();

        // put stuff after ';' over to the side
        // look for ',' separated values
        public MimeTypeList(string mimeType)
        {
            if (mimeType != null)
            {
                mimeType.ToDelimitedArray(',').Each(x =>
                {
                    var type = x.Split(';').First();
                    _mimeTypes.Add(type);
                });
            }
        }

        public void AddMimeType(string mimeType)
        {
            _mimeTypes.Add(mimeType);
        }

        public bool Matches(params string[] mimeTypes)
        {
            return _mimeTypes.Intersect(mimeTypes).Any();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<string> GetEnumerator()
        {
            return _mimeTypes.GetEnumerator();
        }

        
    }
}