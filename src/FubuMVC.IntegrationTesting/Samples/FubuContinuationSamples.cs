using System;
using System.Net;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Katana;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Samples
{
    public class DirectorEndpoint
    {
        // SAMPLE: continuation-usage
        public FubuContinuation get_directions()
        {
            var customer = determineCustomer();
            return customer.IsSpecial()
                ? FubuContinuation.RedirectTo(new SpecialCustomer {Id = customer.Id})
                : FubuContinuation.RedirectTo(new NormalCustomer {Id = customer.Id});
        }

        // ENDSAMPLE

        private Customer determineCustomer()
        {
            throw new System.NotImplementedException();
        }
    }

    public class NormalCustomer
    {
        public Guid Id { get; set; }
    }

    public class SpecialCustomer
    {
        public Guid Id { get; set; }
    }

    public class Customer
    {
        public bool IsSpecial()
        {
            throw new System.NotImplementedException();
        }

        public Guid Id { get; set; }
    }


    [TestFixture]
    public class NumberEndpointTester
    {
        [Test]
        public void try_out_continuations()
        {
            // SAMPLE: fubucontinuation-in-action

            // Proceed as normal through the chain
            TestHost.Scenario(_ => {
                _.Get.Input(new Number { Value = 1 });
                _.ContentShouldBe("The number is 1");
            });

            // 5 is special, "jump the tracks" and execute
            // a different chain inline
            TestHost.Scenario(_ =>
            {
                _.Get.Input(new Number { Value = 5 });
                _.ContentShouldBe("Five is a special number!");
            });

            // You are not authorized to specify a number greater
            // than 10
            TestHost.Scenario(_ =>
            {
                _.Get.Input(new Number { Value = 11 });
                _.StatusCodeShouldBe(HttpStatusCode.Unauthorized);
            });

            // Negative numbers are invalid, so we'll redirect
            // the user to another page instead
            TestHost.Scenario(_ =>
            {
                _.Get.Input(new Number { Value = -1 });
                _.StatusCodeShouldBe(HttpStatusCode.Redirect);
                _.Header(HttpResponseHeaders.Location).SingleValueShouldEqual("/invalid");
            });


            TestHost.Scenario(_ => {
                _.Get.Input(new Number { Value = 2 });
                _.ContentShouldBe("The doubled number is 4");
            });

            TestHost.Scenario(_ => {
                _.Get.Input(new Number { Value = 4 });
                _.StatusCodeShouldBe(HttpStatusCode.Redirect);
                _.Header(HttpResponseHeaders.Location).SingleValueShouldEqual("/doubled/8");
            });
            // ENDSAMPLE
        }
    }


    // SAMPLE: NumberFilter
    public class NumberFilter
    {
        public FubuContinuation Filter(Number number)
        {
            // Issue an HTTP 302 to /invalid
            if (number.Value < 0)
                return FubuContinuation.RedirectTo<NumberEndpoint>(x => x.get_invalid());

            // Render the DoubleNumber{Value=4} resource instead
            if (number.Value == 2)
                return FubuContinuation.TransferTo(new DoubleNumber {Value = 4});

            // Issue an HTTP 302 to /doubled/8
            if (number.Value == 4)
                return FubuContinuation.RedirectTo(new DoubleNumber {Value = 8});

            // Execute a different chain instead
            if (number.Value == 5)
                return FubuContinuation.TransferTo<NumberEndpoint>(x => x.get_special());

            // HTTP 403 and don't proceed any farther
            if (number.Value > 10)
                return FubuContinuation.EndWithStatusCode(HttpStatusCode.Unauthorized);

            return FubuContinuation.NextBehavior();
        }
    }

    // ENDSAMPLE

    // SAMPLE: filter-testing
    [TestFixture]
    public class NumberFilterTester
    {
        [Test]
        public void just_go_on_if_not_a_special_number()
        {
            new NumberFilter()
                .Filter(new Number {Value = 1})
                .AssertWasContinuedToNextBehavior();
        }

        [Test]
        public void stop_if_greater_than_10()
        {
            new NumberFilter()
                .Filter(new Number {Value = 11})
                .AssertWasEndedWithStatusCode(HttpStatusCode.Unauthorized);
        }

        [Test]
        public void redirect_if_a_negative_number()
        {
            new NumberFilter()
                .Filter(new Number {Value = -1})
                .AssertWasRedirectedTo<NumberEndpoint>(x => x.get_invalid());
        }

        [Test]
        public void transfer_to_special_if_5()
        {
            new NumberFilter()
                .Filter(new Number {Value = 5})
                .AssertWasTransferedTo<NumberEndpoint>(x => x.get_special());
        }

        [Test]
        public void transfer_if_2()
        {
            new NumberFilter()
                .Filter(new Number {Value = 2})
                .AssertWasTransferedTo(new DoubleNumber {Value = 4});
        }

        [Test]
        public void redirect_if_4()
        {
            new NumberFilter()
                .Filter(new Number {Value = 4})
                .AssertWasRedirectedTo(new DoubleNumber {Value = 8});
        }
    }

    // ENDSAMPLE

    // SAMPLE: NumberEndpoint
    public class NumberEndpoint
    {
        [Filter(typeof (NumberFilter))]
        public string get_number_Value(Number number)
        {
            return "The number is " + number.Value;
        }

        public string get_doubled_Value(DoubleNumber number)
        {
            return "The doubled number is " + number.Value;
        }

        public string get_special()
        {
            return "Five is a special number!";
        }

        public string get_invalid()
        {
            return "The number is invalid!";
        }
    }

    public class Number
    {
        public int Value { get; set; }
    }

    public class DoubleNumber
    {
        public int Value { get; set; }

        // Note that I had to implement an Equals()
        // method on DoubleNumber for the testing to work
        protected bool Equals(DoubleNumber other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DoubleNumber) obj);
        }

        public override int GetHashCode()
        {
            return Value;
        }
    }

    // ENDSAMPLE


    // SAMPLE: redirectable
    public class RedirectingNumber : Number
    {
    }

    public class RedirectableEndpoint
    {
        public RedirectableNumber get_redirectable(RedirectingNumber number)
        {
            return new RedirectableNumber
            {
                Number = number.Value,
                RedirectTo = number.Value < 0
                    ? FubuContinuation.TransferTo<NumberEndpoint>(x => x.get_invalid())
                    : null
            };
        }
    }

    public class RedirectableNumber : IRedirectable
    {
        public int Number { get; set; }
        public FubuContinuation RedirectTo { get; set; }
    }

    // ENDSAMPLE

    // SAMPLE: offline-filter
    public class OfflineFilter
    {
        private readonly IOfflineDetector _detector;

        // You can use constructor injection from your
        // IoC container for dependencies
        public OfflineFilter(IOfflineDetector detector)
        {
            _detector = detector;
        }

        // ActionFilter methods can accept an input message
        public FubuContinuation Filter()
        {
            return _detector.IsOffline()
                ? FubuContinuation.RedirectTo<OfflineEndpoint>(x => x.get_offline())
                : FubuContinuation.NextBehavior();
        }
    }

    public interface IOfflineDetector
    {
        bool IsOffline();
    }

    public class OfflineEndpoint
    {
        public string get_offline()
        {
            return "We're offline for maintenance for some reason";
        }
    }

    // ENDSAMPLE

    // SAMPLE: offline-filter-policy
    public class OfflineFilterPolicy : Policy
    {
        public OfflineFilterPolicy()
        {
            Where.RespondsToHttpMethod("GET");

            ModifyBy(chain => { chain.InsertFirst(ActionFilter.For<OfflineFilter>(x => x.Filter())); });
        }
    }

    // You would add the Policy above to the application's
    // FubuRegistry class
    public class MyOfflineFubuRegistry : FubuRegistry
    {
        public MyOfflineFubuRegistry()
        {
            Policies.Local.Add<OfflineFilterPolicy>();
        }
    }

    // ENDSAMPLE


    // SAMPLE: continuation-up-the-stack
    public class TransactionalBehavior : WrappingBehavior
    {
        protected override void invoke(Action action)
        {
            using (var tx = start())
            {
                // Call the inner behavior, which itself may
                // be executing a FubuContinuation.TransferTo()
                action();
                tx.Commit();
            }
        }

        private Transaction start()
        {
            return new Transaction();
        }
    }

    // ENDSAMPLE

    public class Transaction : IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Commit()
        {
        }
    }
}