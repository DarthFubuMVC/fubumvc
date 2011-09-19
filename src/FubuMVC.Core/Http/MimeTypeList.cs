using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enumerable = System.Linq.Enumerable;

namespace FubuMVC.Core.Http
{
    public class MimeTypeList : IEnumerable<string>
    {
        private readonly IList<string> _mimeTypes = new List<string>();

        // put stuff after ';' over to the side
        // look for ',' separated values
        public MimeTypeList(string mimeType)
        {
            GenericEnumerableExtensions.Each<string>(mimeType.ToDelimitedArray(','), x =>
            {
                var type = Enumerable.First<string>(x.Split(';'));
                _mimeTypes.Add(type);
            });
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