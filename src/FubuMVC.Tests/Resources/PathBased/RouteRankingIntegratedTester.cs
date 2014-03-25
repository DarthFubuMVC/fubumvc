using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Resources.PathBased
{
	[TestFixture]
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

		[TestFixtureSetUp]
		public void beforeAll()
		{
			var registry = new FubuRegistry();
			registry.Actions.IncludeType<Controller1>();

			_graph = BehaviorGraph.BuildFrom(registry);
		}

		[Test]
		public void rank_me_last_inputs_should_rank_highly()
		{
			_graph.BehaviorFor<Controller1>(x => x.get_last_ranked(null))
                .As<RoutedChain>()
			      .Route.Rank.ShouldEqual(int.MaxValue);
		}

		[Test]
		public void no_route_inputs_should_rank_zero()
		{
			_graph.BehaviorFor<Controller1>(x => x.zero(null))
                .As<RoutedChain>()
				  .Route.Rank.ShouldEqual(0);
		}

		[Test]
		public void one_route_input_should_as_one()
		{
			_graph.BehaviorFor<Controller1>(x => x.one(null))
                .As<RoutedChain>()
			      .Route.Rank.ShouldEqual(1);
		}

		[Test]
		public void two_route_inputs_should_as_one()
		{
			_graph.BehaviorFor<Controller1>(x => x.two(null))
                .As<RoutedChain>()
				  .Route.Rank.ShouldEqual(2);
		}

	}
}