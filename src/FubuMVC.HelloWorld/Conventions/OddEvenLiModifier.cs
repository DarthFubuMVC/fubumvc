using FubuMVC.Core.UI.Configuration;
using FubuMVC.HelloWorld.Controllers.Products;

namespace FubuMVC.HelloWorld.Conventions
{
    public class OddEvenLiModifier : IPartialElementModifier
    {
        private bool matches(AccessorDef accessorDef)
        {
            return accessorDef.ModelType == typeof(ProductsViewModel);
        }

        private EachPartialTagModifier modifier = (request, tag, index, count) =>
                                                      {
                                                          if ((index % 2) == 0)
                                                              tag.AddClass("odd");
                                                          else
                                                              tag.AddClass("even");

                                                          if (index == 0)
                                                              tag.AddClass("first");

                                                          if (index == count - 1)
                                                              tag.AddClass("last");
                                                      };

        public EachPartialTagModifier CreateModifier(AccessorDef accessorDef)
        {
            var something = matches(accessorDef) ? modifier : null;
            return something;

        }
    }
}