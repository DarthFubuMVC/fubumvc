using FubuMVC.Core;
using FubuMVC.Core.Continuations;
using FubuValidation;

namespace FubuMVC.HelloSpark.Controllers
{
    public class ProductsController
    {
        [UrlPattern("products/create")]
        public CreateProductModel Create(CreateProductRequest request)
        {
            return new CreateProductModel();
        }

        [UrlPattern("products/list")]
        public ProductsListModel List(ProductsListRequest request)
        {
            return new ProductsListModel();
        }

        [UrlPattern("products/new")]
        public FubuContinuation New(CreateProductInput input)
        {
            return FubuContinuation.RedirectTo(new ProductsListRequest());
        }
    }

    public class CreateProductRequest
    {
    }

    public class ProductsListRequest
    {
    }

    public class CreateProductModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
    }

    public class CreateProductInput : CreateProductModel
    {
    }

    public class ProductsListModel
    {
    }
}