using System;
using FubuCore;
using HtmlTags.Conventions;

namespace FubuHtml.Elements
{
    public abstract class TagRequestActivator<T> : ITagRequestActivator where T : TagRequest
    {
        // TODO -- move this to HtmlTags

        public bool Matches(Type requestType)
        {
            return requestType.CanBeCastTo<T>();
        }

        public void Activate(TagRequest request)
        {
            Activate(request.As<T>());
        }

        public abstract void Activate(T request);
    }
}