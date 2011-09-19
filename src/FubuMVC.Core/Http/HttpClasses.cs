using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace FubuMVC.Core.Http
{
    public interface IRequestConditional
    {
        bool Matches(string mimeType);
    }

    public class MimeTypeList : IEnumerable<string>
    {
        private readonly IList<string> _mimeTypes = new List<string>();

        // put stuff after ';' over to the side
        // look for ',' separated values
        public MimeTypeList(string mimeType)
        {
            mimeType.ToDelimitedArray(',').Each(x =>
            {
                var type = x.Split(';').First();
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

    // TODO -- really, really have to make sure this works in model binding
    // cause it doesn't right now
    public class CurrentMimeType
    {
        public MimeTypeList ContentType { get; set; }
        public MimeTypeList AcceptTypes { get; set; }
    }


}