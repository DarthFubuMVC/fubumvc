using System.Collections.Generic;
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


        public ProductsDisplayViewModel Command(ProductsForm form)
        {
            return new ProductsDisplayViewModel
            {
                Products = form.Products,
            };
        }
    }

    public class ProductsForm
    {
        public IList<Product> Products { get; set; }
    }

    public class ProductsViewModel
    {
        public IList<Product> Products { get; set; }
    }

    public class ProductsDisplayViewModel
    {
        public IList<Product> Products { get; set; }
    }

    public class ProductsRequest
    {
    }

    public class Product
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public IList<ProductPart> Parts { get; set; }
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

    public class ProductsDisplayView : FubuPage<ProductsDisplayViewModel>
    {
    }

    public class ProductPartial : FubuControl<Product>
    {
    }

    public class ProductDisplayPartial : FubuControl<Product>
    {
    }

    public class PartPartial : FubuControl<ProductPart>
    {
    }

    public class PartDisplayPartial : FubuControl<ProductPart>
    {
    }
}