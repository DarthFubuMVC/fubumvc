using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Diagnostics.Navigation;

namespace FubuMVC.GettingStarted.Intro
{
    public class GettingStartedController
    {
         public GettingStartedViewModel Execute(GettingStartedRequestModel request)
         {
             return new GettingStartedViewModel();
         }
    }

    public class GettingStartedNavigationItem : NavigationItemBase
    {
        public GettingStartedNavigationItem(BehaviorGraph graph, IEndpointService endpointService) 
            : base(graph, endpointService)
        {
        }

        public override string Text()
        {
            return "Getting Started";
        }

        public override int Rank
        {
            get { return int.MinValue + 1; }
        }

        protected override object inputModel()
        {
            return new GettingStartedRequestModel();
        }
    }

    public class GettingStartedRequestModel { }

    public class GettingStartedViewModel { }
}