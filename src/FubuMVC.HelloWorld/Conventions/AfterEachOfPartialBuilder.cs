using FubuMVC.HelloWorld.Controllers.Products;
using FubuMVC.UI.Configuration;
using HtmlTags;

namespace FubuMVC.HelloWorld.Conventions
{
    public class AfterEachOfPartialBuilder : EachOfPartialBuilder
    {
        protected override bool matches(AccessorDef def)
        {
            return def.ModelType == typeof(ProductsViewModel);
        }

        public override HtmlTag Build(ElementRequest request, int index, int total)
        {
            return new HtmlTag("/li").NoClosingTag();
        }
    }
}