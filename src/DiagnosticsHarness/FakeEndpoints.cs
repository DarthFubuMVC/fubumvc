using System;
using System.Threading;
using FubuMVC.Core;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Continuations;

namespace DiagnosticsHarness
{
   
    public class FakeEndpoints
    {
        [WrapWith(typeof(DelayWrapper))]
        public string get_hello()
        {
            return "Hello";
        }

        [WrapWith(typeof(DelayWrapper))]
        public string get_failure()
        {
            throw new Exception("This failed!");
        }

        [WrapWith(typeof(DelayWrapper))]
        public Team get_team_favorite()
        {
            return new Team
            {
                City = "Kansas City",
                Mascot = "Chiefs"
            };
        }

        public FubuContinuation get_redirects()
        {
            return FubuContinuation.RedirectTo<FakeEndpoints>(x => x.get_hello());
        }

        [WrapWith(typeof(DelayWrapper))]
        public string post_team(Team team)
        {
            return team.City + " " + team.Mascot;
        }

        [WrapWith(typeof(DelayWrapper))]
        public AjaxContinuation delete_team(Team team)
        {
            return AjaxContinuation.Successful();
        }

        [WrapWith(typeof(DelayWrapper))]
        public AjaxContinuation put_team(Team team)
        {
            return AjaxContinuation.Successful();
        }
    }

    public class DelayWrapper : WrappingBehavior
    {
        private static readonly Random random = new Random();

        protected override void invoke(Action action)
        {
            var ms = random.NextDouble() * 1000;
            Thread.Sleep(TimeSpan.FromMilliseconds(ms));

            action();
        }
    }

    public class Team
    {
        public string City { get; set; }
        public string Mascot { get; set; }
    }
}