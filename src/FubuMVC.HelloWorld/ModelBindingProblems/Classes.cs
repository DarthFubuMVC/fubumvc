using System.Collections.Generic;

namespace FubuMVC.HelloWorld.ModelBindingProblems
{
    public class HomeViewModel
    {
        public Customer Customer { get; set; }
        public IEnumerable<Order> Orders { get; set; }
        public string Request { get; set; }
    }

    public class Customer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class Order
    {
        public int Quantity { get; set; }
        public string Product { get; set; }
    }

    public class HomeViewModelController
    {
        public HomeViewModel get_home_model()
        {
            return new HomeViewModel(){
                Customer = new Customer(){
                    FirstName = "Jeremy",
                    LastName = "Miller"
                }
            };
        }

        public HomeViewModel post_home_model(HomeViewModel model)
        {
            return model;
        }
    }
}