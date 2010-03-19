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
                                   new Product{Code="1111", Name="TV Remote Control", Parts = GetParts("1111")},
                                   new Product{Code="2222", Name="DVR Unit", Parts = GetParts("2222")},
                                   new Product{Code="3333", Name="HD Antenna", Parts = GetParts("3333")}
                               };

            return new ProductsViewModel{ Products = products };
        }

        private static ProductPart[] GetParts(string numberRoot)
        {
            return new[]
                       {
                           new ProductPart(numberRoot + "- 1"),
                           new ProductPart(numberRoot + "- 2"),
                           new ProductPart(numberRoot + "- 3")
                       };
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
        public ProductPart[] Parts { get; set; }
    }

    public class ProductPart
    {
        public ProductPart(){}

        public ProductPart(string num)
        {
            PartNum = num;
        }

        public string PartNum { get; set; }
    }

    public class ProductsView : FubuPage<ProductsViewModel>
    {
    }

    public class ProductPartial : FubuControl<Product>
    {
    }

    public class PartPartial : FubuControl<ProductPart>
    {
    }
}