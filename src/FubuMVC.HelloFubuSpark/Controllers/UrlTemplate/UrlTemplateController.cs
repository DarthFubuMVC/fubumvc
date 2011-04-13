using FubuMVC.Core;
using FubuMVC.HelloFubuSpark.Controllers.Products;

namespace FubuMVC.HelloFubuSpark.Controllers.UrlTemplate
{
    public class UrlTemplateController
    {
        public UrlTemplateViewModel Query(UrlTemplateRequest query)
        {
             var products = new[]
                               {
                                   new Product{Code="1111", Name="TV Remote Control", Parts = GetParts("1111")},
                                   new Product{Code="2222", Name="DVR Unit", Parts = GetParts("2222")},
                                   new Product{Code="3333", Name="HD Antenna", Parts = GetParts("3333")}
                               };

            return new UrlTemplateViewModel()
                       {
                           Products = products
                       };
        }

        public ViewProductViewModel Find(ViewProductRequest model)
        {
            return new ViewProductViewModel()
                       {
                           Product = new Product() {Code = model.Code}
                       };
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
    }

    public class ViewProductViewModel
    {
        public Product Product { get; set; }
    }

    public class ViewProductRequest
    {
        [RouteInput]
        public string Code { get; set; }
    }

    public class UrlTemplateViewModel
    {
        public Product[] Products { get; set; }
    }

    public class UrlTemplateRequest
    {
    }
}