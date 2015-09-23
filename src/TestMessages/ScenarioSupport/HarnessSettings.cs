using System;

namespace TestMessages.ScenarioSupport
{
    public class HarnessSettings
    {
        public Uri Website1 { get; set; }
        public Uri Website2 { get; set; }
        public Uri Website3 { get; set; }
        public Uri Website4 { get; set; }


        public Uri Service1 { get; set; }
        public Uri Service2 { get; set; }
        public Uri Service3 { get; set; }
        public Uri Service4 { get; set; }
    }

    /*
     * 
     * 
Send a message that raises events
    Website1
        Listens to memory://harness/website1 with 
        Listens to memory://localhost/Website1/replies with 
        Publishes to Harness:Service1
            * OneMessage
        Handles Message, JobRequest`1, JobRequest`1, JobRequest`1, JobRequest`1, SubscriptionRequested, SubscriptionsChanged, SubscriptionsRemoved, TakeOwnershipRequest, TaskHealthRequest, TaskDeactivation

    Service1
        Listens to memory://harness/service1 with 
        Listens to memory://localhost/Service1/replies with 
        Publishes to Harness:Service1
            * TwoMessage
            * ThreeMessage
        Handles OneMessage, TwoMessage, ThreeMessage, Message, JobRequest`1, JobRequest`1, JobRequest`1, JobRequest`1, SubscriptionRequested, SubscriptionsChanged, SubscriptionsRemoved, TakeOwnershipRequest, TaskHealthRequest, TaskDeactivation
        Handling OneMessage raises TwoMessage
        Handling OneMessage raises ThreeMessage


    Actions
        Node Website1 sends new message of type OneMessage (original message)
    Messages are all accounted for

    Assertions
        Message of type OneMessage (original message) should be received by Service1
        Expecting message of type TwoMessage to be received by node Service1 as a result of message of type OneMessage (original message) being handled
        Expecting message of type ThreeMessage to be received by node Service1 as a result of message of type OneMessage (original message) being handled

    Messages Received
    * OneMessage received by memory://harness/service1
    * TwoMessage received by memory://harness/service1
    * ThreeMessage received by memory://harness/service1
     * 
     * 
     * 
     * 
     */
}