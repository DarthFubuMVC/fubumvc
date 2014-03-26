using System;
using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.UI.Elements.Builders;
using FubuMVC.Core.UI.Forms;
using HtmlTags;
using HtmlTags.Conventions;
using FubuCore;

namespace FubuMVC.Core.UI
{
    public class HtmlConventionRegistry : ProfileExpression, IFubuRegistryExtension
    {
        private readonly HtmlConventionLibrary _library;
        private readonly Lazy<ProfileExpression> _templates; 

        public HtmlConventionRegistry() : this(new HtmlConventionLibrary()){}

        private HtmlConventionRegistry(HtmlConventionLibrary library) : base(library, TagConstants.Default)
        {
            _library = library;
            _templates = new Lazy<ProfileExpression>(() => new ProfileExpression(_library, ElementConstants.Templates));
        }

        public HtmlConventionLibrary Library
        {
            get { return _library; }
        }

        public void Profile(string profileName, Action<ProfileExpression> configure)
        {
            var expression = new ProfileExpression(_library, profileName);
            configure(expression);
        }

        public ProfileExpression Templates
        {
            get { return _templates.Value; }
        }

        void IFubuRegistryExtension.Configure(FubuRegistry registry)
        {
            registry.AlterSettings<HtmlConventionLibrary>(library => library.Import(_library));
        }

        public TagLibrary<FormRequest> Forms
        {
            get { return _library.For<FormRequest>(); }
        } 
    }

    public class ElementActionExpression
    {
        private readonly BuilderSet<ElementRequest> _set;
        private readonly Func<ElementRequest, bool> _filter;
        private readonly string _filterDescription;

        public ElementActionExpression(BuilderSet<ElementRequest> set, Func<ElementRequest, bool> filter, string filterDescription)
        {
            _set = set;
            _filter = filter;
            _filterDescription = filterDescription.IsNotEmpty() ? filterDescription : "User Defined";
        }

        public void BuildBy(IElementBuilder builder)
        {
            var conditionalBuilder = new ConditionalElementBuilder(_filter, builder){
                ConditionDescription = _filterDescription
            };

            _set.Add(conditionalBuilder);
        }

        public void BuildBy<T>() where T : IElementBuilder, new()
        {
            BuildBy(new T());
        }

        public void BuildBy(Func<ElementRequest, HtmlTag> tagBuilder, string description = null)
        {
            var builder = new LambdaElementBuilder(_filter, tagBuilder){
                ConditionDescription = _filterDescription,
                BuilderDescription = description ?? "User Defined"
            };

            _set.Add(builder);
        }

        public void ModifyWith(IElementModifier modifier)
        {
            var conditionalModifier = new ConditionalElementModifier(_filter, modifier){
                ConditionDescription = _filterDescription
            };

            _set.Add(conditionalModifier);
        }

        public void ModifyWith<T>() where T : IElementModifier, new()
        {
            ModifyWith(new T());
        }

        public void ModifyWith(Action<ElementRequest> modification, string description = null)
        {
            var modifier = new LambdaElementModifier(_filter, modification){
                ConditionDescription = _filterDescription,
                ModifierDescription = description ?? "User Defined"
            };

            _set.Add(modifier);
        }

        public void Attr(string attName, object value)
        {
            ModifyWith(req => req.CurrentTag.Attr(attName, value), "Set @{0} = '{1}'".ToFormat(attName, value));
        }

        public void AddClass(string className)
        {
            ModifyWith(req => req.CurrentTag.AddClass(className), "Add class '{0}'".ToFormat(className));
        }
    }
}