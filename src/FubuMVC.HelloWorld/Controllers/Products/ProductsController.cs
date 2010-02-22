using FubuMVC.Core;
using FubuMVC.Core.View;

namespace FubuMVC.HelloWorld.Controllers.Products
{
    public class ProductsController
    {
        public ProductsViewModel Query(ProductsRequest query)
        {
            var products = new[]
                               {
                                   new Product{Code="1111", Name="TV Remote Control"},
                                   new Product{Code="2222", Name="DVR Unit"},
                                   new Product{Code="3333", Name="HD Antenna"}
                               };

            return new ProductsViewModel{ Products = products };
        }

        public ProductsViewModel Command(ProductsForm form)
        {
            return new ProductsViewModel();
        }
    }

    public class ProductsForm
    {
        public Product[] Products { get; set; }
    }

    public class ProductsViewModel
    {
        public Product[] Products { get; set; }
    }

    public class ProductsRequest
    {
    }

    public class Product
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class ProductsView : FubuPage<ProductsViewModel>
    {
    }

    public class ProductPartial : FubuControl<Product>
    {
    }
}