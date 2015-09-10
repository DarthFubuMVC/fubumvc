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
            Raw = mimeType;

            if (mimeType != null)
            {
                mimeType.ToDelimitedArray(',').Each(x =>
                {
                    var type = x.Split(';').First();
                    if (type.IsNotEmpty())
                    {
                        _mimeTypes.Add(type);
                    }
                });
            }
        }

        public string Raw { get; private set; }

        public MimeTypeList(params MimeType[] mimeTypes)
        {
            Raw = mimeTypes.Select(x => x.Value).Join(";");
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

        public override string ToString()
        {
            return _mimeTypes.Join(", ");
        }
    }
}