using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.UI.Elements;
using HtmlTags;
using HtmlTags.Conventions;
using System.Linq;

namespace FubuMVC.Core.UI.Templates
{
    // TODO -- add a feature to add partials as templates?
    public interface ITemplateWriter
    {
        void AddTemplate(string subject, HtmlTag tag);
        void AddTemplate(string subject, string html);
        void AddElement(Accessor accessor, string category);
        void DisplayFor<T>(Expression<Func<T, object>> property);
        void InputFor<T>(Expression<Func<T, object>> property);
        void LabelFor<T>(Expression<Func<T, object>> property) where T : class;
        HtmlTag WriteAll();
    }

    public class TemplateWriter : ITemplateWriter
    {
        private readonly ITagRequestActivator[] _activators;
        private readonly Lazy<ITagGenerator<ElementRequest>> _elements;
        private readonly HtmlConventionLibrary _library;
        private readonly IList<HtmlTag> _tags = new List<HtmlTag>();

        public TemplateWriter(ActiveProfile profile, HtmlConventionLibrary library, ITagRequestBuilder tagRequestBuilder)
        {
            _library = library;
            var factory = new TagGeneratorFactory(profile, library, tagRequestBuilder);
            _elements = new Lazy<ITagGenerator<ElementRequest>>(factory.GeneratorFor<ElementRequest>);
        }

        public static string SubjectFor(Accessor accessor, string category)
        {
            return "{0}-{1}-{2}".ToFormat(category.ToLower(), accessor.OwnerType.Name, accessor.Name);
        }

        #region ITemplateWriter Members

        public void AddTemplate(string subject, HtmlTag tag)
        {
            var subjectTag = new HtmlTag("div").Attr("data-subject", subject).Append(tag);
            _tags.Add(subjectTag);
            
        }

        public void AddTemplate(string subject, string html)
        {
            AddTemplate(subject, new LiteralTag(html));
        }

        public void AddElement(Accessor accessor, string category)
        {
            var request = new ElementRequest(accessor);
            HtmlTag tag = _elements.Value.Build(request, category: category,
                                                profile: ElementConstants.Templates);

            string subject = SubjectFor(accessor, category);
            AddTemplate(subject, tag);
        }

        public void DisplayFor<T>(Expression<Func<T, object>> property)
        {
            AddElement(property.ToAccessor(), ElementConstants.Display);
        }

        public void InputFor<T>(Expression<Func<T, object>> property)
        {
            AddElement(property.ToAccessor(), ElementConstants.Editor);
        }

        public void LabelFor<T>(Expression<Func<T, object>> property) where T : class
        {
            AddElement(property.ToAccessor(), ElementConstants.Label);
        }

        public HtmlTag WriteAll()
        {
            if (!_tags.Any())
            {
                return new HtmlTag("div").Render(false);
            }

            var tag = new HtmlTag("div").AddClass("templates").Hide();
            tag.Append(_tags);

            _tags.Clear();

            return tag;
        }

        #endregion
    }
}