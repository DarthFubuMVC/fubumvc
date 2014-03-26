using HtmlTags.Conventions;

namespace FubuMVC.Core.UI.Elements
{
    public interface IElementBuilder : ITagBuilder<ElementRequest>
    {
        
    }

    public interface IElementBuilderPolicy : ITagBuilderPolicy<ElementRequest>
    {
        
    }

    public abstract class ElementTagBuilder : TagBuilder<ElementRequest>, IElementBuilderPolicy, IElementBuilder
    {
        
    }
}