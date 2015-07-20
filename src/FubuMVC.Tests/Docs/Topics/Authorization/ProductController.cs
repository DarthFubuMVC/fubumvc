using FubuMVC.Core.Security;
using FubuMVC.Core.Security.Authorization;

namespace FubuMVC.Tests.Docs.Topics.Authorization
{
    // SAMPLE: authorization-authorizedby
    public class ProductController
    {
        [AuthorizedBy(typeof(PurchasedProductPolicy))]
        public void SaveReview(ReviewInputModel input)
        {
            //logic
        }
    }
    // ENDSAMPLE

    public class ReviewInputModel
    {
    }
}