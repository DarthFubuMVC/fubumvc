using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Resources.PathBased
{
	
	public class RouteRankingIntegratedTester
	{
		private BehaviorGraph _graph;

		public class Controller1
		{
			public string get_last_ranked(RankedLastRequest request)
			{
				return null;
			}

			public string zero(NormalRequest request)
			{
				return null;
			}

			public string one(OneInputRequest request)
			{
				return null;
			}

			public string two(TwoInputsRequest request)
			{
				return null;
			}

		}

		public class RankedLastRequest : IRankMeLast  { }
		public class NormalRequest  { }
		public class OneInputRequest
		{
			[RouteInput]
			public string First { get; set; }
		}

		public class TwoInputsRequest
		{
			[RouteInput]
			public string First { get; set; }
			[RouteInput]
			public string Second { get; set; }
		}

	    public RouteRankingIntegratedTester()
	    {
            var registry = new FubuRegistry();
            registry.Actions.IncludeType<Controller1>();

            _graph = BehaviorGraph.BuildFrom(registry);
        }


		[Fact]
		public void rank_me_last_inputs_should_rank_highly()
		{
			_graph.ChainFor<Controller1>(x => x.get_last_ranked(null))
                .As<RoutedChain>()
			      .Route.Rank.ShouldBe(int.MaxValue);
		}

		[Fact]
		public void no_route_inputs_should_rank_zero()
		{
			_graph.ChainFor<Controller1>(x => x.zero(null))
                .As<RoutedChain>()
				  .Route.Rank.ShouldBe(0);
		}

		[Fact]
		public void one_route_input_should_as_one()
		{
			_graph.ChainFor<Controller1>(x => x.one(null))
                .As<RoutedChain>()
			      .Route.Rank.ShouldBe(1);
		}

		[Fact]
		public void two_route_inputs_should_as_one()
		{
			_graph.ChainFor<Controller1>(x => x.two(null))
                .As<RoutedChain>()
				  .Route.Rank.ShouldBe(2);
		}

	}
}