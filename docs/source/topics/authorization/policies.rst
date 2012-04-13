===================================
Working with authorization policies
===================================

There may be situations where determining authorization based on role is not
enough.  FubuMVC provides ways to define custom rules that can easily be hooked
into the authorization pipeline.

Rolling with a custom authorization policy
------------------------------------------

When will defining a custom policy be useful?  Let's say, for example, a
customer can only post a review for products they have purchased in the past.
It would make sense to look at the purchase history of the customer to determine
if they are authorized to make a review. 

The policy will retrieve a customer's purchase history and determine if any of
the purchases previously made contains the product in question.  If the customer
has not yet made the purchase, they will be denied to execute the action;
otherwise, they will be allowed to proceed.

.. code-block:: c#

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

Wiring it up
------------

One way of wiring it up is to use the AuthorizedByAttribute, which takes in
array of policy types.  If any of the types do not implement
IAuthorizationPolicy, then an exception will be thrown.

.. note::
  The AuthorizedByAttribute also allows for types that implement
  IAuthorizationRule<> which will be discussed later.

.. note::
  The attribute can be applied at class level as well.

Example usage:

.. code-block:: c#

  public class ProductController
  {
      [AuthorizedBy(typeof(PurchasedProductPolicy))]
      public void SaveReview(ReviewInputModel input)
      {
          //logic
      }
  }

Of course, the more preferred way of doing things is to apply it as a convention
through a behavior chain policy.

.. code-block:: c#

  public class PurchasedProductAuthorizationPolicy : IConfigurationAction
  {
      public void Configure(BehaviorGraph graph)
      {
          graph.Actions()
              .Where(x => /*convention predicate*/)
              .Each(x =>
              {
                  x.ParentChain().Authorization.AddPolicy(roleName);
              });
      }
  }
