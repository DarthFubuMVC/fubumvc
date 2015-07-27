using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuCore.Util;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Monitoring;

namespace FubuMVC.Tests.ServiceBus.Monitoring
{
    public class FakeTransportPeer : ITransportPeer
    {
        public FakeTransportPeer(string nodeId, string machineName, Uri controlChannel)
        {
            NodeId = nodeId;
            MachineName = machineName;
            ControlChannel = controlChannel;
        }

        public readonly Cache<Uri, OwnershipStatus> OwnershipResults =
            new Cache<Uri, OwnershipStatus>(_ => OwnershipStatus.OwnershipActivated);

        public readonly Cache<Uri, HealthStatus> SubjectStatus =
            new Cache<Uri, HealthStatus>(_ => HealthStatus.Inactive);

        public Task<OwnershipStatus> TakeOwnership(Uri subject)
        {
            OwnedSubjects.Fill(subject);
            return OwnershipResults[subject].ToCompletionTask();
        }


        public Task<TaskHealthResponse> CheckStatusOfOwnedTasks()
        {
            var response = new TaskHealthResponse
            {
                Tasks = OwnedSubjects.Select(uri => new PersistentTaskStatus
                {
                    Status = SubjectStatus[uri],
                    Subject = uri
                }).ToArray()
            };

            return response.ToCompletionTask();
        }

        public void RemoveOwnershipFromNode(IEnumerable<Uri> subjects)
        {
            throw new NotImplementedException();
        }

        public IList<Uri> OwnedSubjects = new List<Uri>();

        public IEnumerable<Uri> CurrentlyOwnedSubjects()
        {
            return OwnedSubjects;
        }

        public string NodeId { get; private set; }
        public string MachineName { get; private set; }
        public Uri ControlChannel { get; private set; }

        public Task<bool> Deactivate(Uri subject)
        {
            OwnedSubjects.Remove(subject);

            return Task.FromResult(true);
        }

        public void IsSuccessfullyRunning(Uri subject)
        {
            SubjectStatus[subject] = HealthStatus.Active;
            OwnedSubjects.Fill(subject);
            OwnershipResults[subject] = OwnershipStatus.OwnershipActivated;
        }

        public void OwnsButIsInState(Uri subject, HealthStatus status)
        {
            OwnedSubjects.Fill(subject);
            SubjectStatus[subject] = status;
        }
    }
}