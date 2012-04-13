using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Runtime;

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

        public MimeTypeList(params MimeType[] mimeTypes)
        {
            _mimeTypes.AddRange(mimeTypes.Select(x => x.Value));
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