using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Monitoring;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Monitoring
{
    [TestFixture]
    public class OrderedAssignmentTester
    {
        public ITransportPeer route(params RiggedTransportPeer[] peers)
        {
            var assignment = new OrderedAssignment("foo://1".ToUri(), peers);
            var task = assignment.SelectOwner();

            task.Wait();

            return task.Result;
        }

        [Test]
        public void only_one_happy_path()
        {
            route(new RiggedTransportPeer("A", OwnershipStatus.OwnershipActivated))
                .NodeId.ShouldBe("A");

            route(new RiggedTransportPeer("B", OwnershipStatus.AlreadyOwned))
                .NodeId.ShouldBe("B");
        }

        [Test]
        public void selects_the_first_good_peer_in_order()
        {
            route(
                new RiggedTransportPeer("A", OwnershipStatus.OwnershipActivated),
                new RiggedTransportPeer("B", OwnershipStatus.OwnershipActivated),
                new RiggedTransportPeer("C", OwnershipStatus.OwnershipActivated)
                )
                .NodeId.ShouldBe("A");
        }

        [Test]
        public void passes_on_peer_that_does_not_know_the_subject()
        {
            route(
                new RiggedTransportPeer("A", OwnershipStatus.UnknownSubject),
                new RiggedTransportPeer("B", OwnershipStatus.OwnershipActivated),
                new RiggedTransportPeer("C", OwnershipStatus.OwnershipActivated)
                )
                .NodeId.ShouldBe("B");
        }

        [Test]
        public void passes_on_peer_that_fails_to_start_the_subject()
        {
            route(
                new RiggedTransportPeer("A", OwnershipStatus.Exception),
                new RiggedTransportPeer("B", OwnershipStatus.OwnershipActivated),
                new RiggedTransportPeer("C", OwnershipStatus.OwnershipActivated)
                )
                .NodeId.ShouldBe("B");
        }

        [Test]
        public void passes_on_peer_that_fails_to_start_the_subject_because_of_timeout()
        {
            route(
                new RiggedTransportPeer("A", OwnershipStatus.TimedOut),
                new RiggedTransportPeer("B", OwnershipStatus.OwnershipActivated),
                new RiggedTransportPeer("C", OwnershipStatus.OwnershipActivated)
                )
                .NodeId.ShouldBe("B");
        }

        [Test]
        public void returns_none_if_none_can_be_assigned()
        {
            route(
                new RiggedTransportPeer("A", OwnershipStatus.TimedOut),
                new RiggedTransportPeer("B", OwnershipStatus.Exception),
                new RiggedTransportPeer("C", OwnershipStatus.TimedOut)
                ).ShouldBeNull();
        }
    }

    public class RiggedTransportPeer : ITransportPeer
    {
        public RiggedTransportPeer(string nodeId, OwnershipStatus status)
        {
            NodeId = nodeId;
            StatusToReturn = status;
        }

        public OwnershipStatus StatusToReturn = OwnershipStatus.Exception;

        public Task<OwnershipStatus> TakeOwnership(Uri subject)
        {
            return Task.Factory.StartNew(() => {
                Thread.Sleep(10);
                return StatusToReturn;
            });
        }

        public Task<TaskHealthResponse> CheckStatusOfOwnedTasks()
        {
            throw new NotImplementedException();
        }

        public void RemoveOwnershipFromNode(IEnumerable<Uri> subjects)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Uri> CurrentlyOwnedSubjects()
        {
            throw new NotImplementedException();
        }

        public string NodeId { get; set; }
        public string MachineName { get; private set; }
        public Uri ControlChannel { get; private set; }
        public Task<bool> Deactivate(Uri subject)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return string.Format("NodeId: {0}", NodeId);
        }
    }
}