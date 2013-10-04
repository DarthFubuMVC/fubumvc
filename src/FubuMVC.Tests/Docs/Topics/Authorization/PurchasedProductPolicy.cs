using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;

namespace FubuMVC.Tests.Docs.Topics.Authorization
{
    // SAMPLE: authorization-policy
    public class PurchasedProductPolicy : IAuthorizationPolicy
    {
        readonly IRepository _repository;

        public PurchasedProductPolicy(IRepository repository)
        {
            _repository = repository;
        }

        public AuthorizationRight RightsFor(IFubuRequest request)
        {
            var customerId = request.Get<Customer>().Id;
            var productId = request.Get<Product>().Id;

            var hasPurchasedProduct = _repository.Get<IPurchaseHistory>(customerId)
                .Any(x => x.ContainsProduct(productId));

            return !hasPurchasedProduct ? AuthorizationRight.Deny : AuthorizationRight.Allow;
        }
    }
    // ENDSAMPLE

    public class Customer
    {
        public object Id { get; set; }
    }

    public class Product
    {
        public object Id { get; set; }
    }

    public interface IRepository
    {
        IEnumerable<IPurchaseHistory> Get<T>(object customerId);
    }

    public interface IPurchaseHistory
    {
        bool ContainsProduct(object productId);
    }
}