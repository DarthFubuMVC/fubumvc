using FubuMVC.Core;
using FubuMVC.GuideApp.Behaviors;

namespace FubuMVC.GuideApp.Examples
{
    public class MyProjectFubuRegistry : FubuRegistry
    {
        public MyProjectFubuRegistry()
        {
			Policies.WrapBehaviorChainsWith<InnerMostBehavior>() 
					.WrapBehaviorChainsWith<MiddleBehavior>()
					.WrapBehaviorChainsWith<OuterBehavior>(); 
					
			Policies.EnrichCallsWith<DemoBehaviorForSelectActions>(
						call => call.Method.Name == "Home" ); 

			Policies.EnrichCallsWith<DemoBehaviorForSelectActions>( 
						call => call.Returns<HomeViewModel>() ); 

			Policies.Add<DemoBehaviorPolicy>(); 
        }
    }
    
    public class InnerMostBehavior : IActionBehavior {}
    
    public class MiddleBehavior : IActionBehavior {}
    
    public class OuterBehavior : IActionBehavior {}
}
