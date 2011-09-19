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

    public class MimetypeRequestConditional : IRequestConditional
    {
        private readonly string[] _mimeTypes;

        public MimetypeRequestConditional(params string[] mimeTypes)
        {
            _mimeTypes = mimeTypes;
        }

        public bool Matches(string mimeType)
        {
            return _mimeTypes.Contains(mimeType);
        }
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
        public CurrentMimeType()
        {
        }

        public CurrentMimeType(string contentType, string acceptType)
        {
            ContentType = new MimeTypeList(contentType);
            AcceptTypes = new MimeTypeList(acceptType);
        }

        public MimeTypeList ContentType { get; set; }
        public MimeTypeList AcceptTypes { get; set; }
    }


}