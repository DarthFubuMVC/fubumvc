using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.UI.Forms;
using HtmlTags.Conventions;

namespace FubuMVC.Core.UI
{
    public class ProfileExpression
    {
        private readonly HtmlConventionLibrary _library;
        private readonly string _profileName;

        public ProfileExpression(HtmlConventionLibrary library, string profileName)
        {
            _library = library;
            _profileName = profileName;
        }

        private BuilderSet<ElementRequest> buildersFor(string category)
        {
            return _library.For<ElementRequest>().Category(category).Profile(_profileName);
        }

        public ElementCategoryExpression Labels
        {
            get
            {
                return new ElementCategoryExpression(buildersFor(ElementConstants.Label));
            }
        }

        public ElementCategoryExpression Displays
        {
            get
            {
                return new ElementCategoryExpression(buildersFor(ElementConstants.Display));
            }
        }

        public ElementCategoryExpression Editors
        {
            get
            {
                return new ElementCategoryExpression(buildersFor(ElementConstants.Editor));
            }
        }

        //public void BuilderPolicy<T>(ITagBuilder<T> builder) where T : TagRequest
        //{

        //}

        //public void Modifier<T>(ITagModifier<T> modifier) where T : TagRequest
        //{
            
        //}
        public void FieldChrome<T>() where T : IFieldChrome, new()
        {
            _library.RegisterService<IFieldChrome, T>(_profileName);
        }
    }
}