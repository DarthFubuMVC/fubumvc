using FubuMVC.Core;
using FubuMVC.Core.Continuations;

namespace FubuMVC.HelloWorld.Controllers.Products
{
    public class DifferentProductsController
    {
        [UrlPattern("differentproducts/create")]
        public CreateDifferentProductModel Create(CreateDifferentProductRequest request)
        {
            return new CreateDifferentProductModel();
        }

        [UrlPattern("differentproducts/list")]
        public DifferentProductsListModel List(DifferentProductsListRequest request)
        {
            return new DifferentProductsListModel();
        }

        [UrlPattern("differentproducts/new")]
        public FubuContinuation New(CreateDifferentProductInput input)
        {
            return FubuContinuation.RedirectTo(new DifferentProductsListRequest());
        }
    }

    public class CreateDifferentProductRequest
    {
    }

    public class DifferentProductsListRequest
    {
    }

    public class CreateDifferentProductModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class CreateDifferentProductInput : CreateDifferentProductModel
    {
    }

    public class DifferentProductsListModel
    {
    }
}