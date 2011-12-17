using System;
using System.Collections.Generic;

namespace FubuMVC.Razor
{
    public class RazorViewDescriptor
    {
        public RazorViewDescriptor()
        {
            Templates = new List<string>();
            Accessors = new List<Accessor>();
        }

        public string TargetNamespace { get; set; }
        public IList<string> Templates { get; set; }
        public IList<Accessor> Accessors { get; set; }

        public class Accessor
        {
            public string Property { get; set; }
            public string GetValue { get; set; }
        }

        public RazorViewDescriptor SetTargetNamespace(string targetNamespace)
        {
            TargetNamespace = targetNamespace;
            return this;
        }

        public RazorViewDescriptor AddTemplate(string template)
        {
            Templates.Add(template);
            return this;
        }

        public RazorViewDescriptor AddAccessor(string property, string getValue)
        {
            Accessors.Add(new Accessor {Property = property, GetValue = getValue});
            return this;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;

            hashCode ^= (TargetNamespace ?? "").GetHashCode();

            foreach (var template in Templates)
                hashCode ^= template.ToLowerInvariant().GetHashCode();

            return hashCode;
        }

        public override bool Equals(object obj)
        {
            var that = obj as RazorViewDescriptor;

            if (that == null || GetType() != that.GetType())
                return false;

            if (!string.Equals(TargetNamespace ?? "", that.TargetNamespace ?? "") ||
                Templates.Count != that.Templates.Count)
            {
                return false;
            }

            for (var index = 0; index != Templates.Count; ++index)
            {
                if (!string.Equals(Templates[index], that.Templates[index], StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
            }
            return true;
        }
    }
}