# Multiple nodes of global subscriptions does not duplicate entries

-> id = 98632634-0751-4ee5-965e-62fbf3d3ed99
-> lifecycle = Acceptance
-> max-retries = 0
-> last-updated = 2015-09-11T00:00:00.0000000
-> tags = 

[Subscriptions]
|> LoadNode Key=Publisher, Registry=PublishingRegistry, ReplyUri=memory://publisher1
|> LoadNode Key=Subscriber, ReplyUri=memory://subscriber1
``` Registry
HasGlobalSubscriptionsRegistry
```

|> LoadNode Key=Subscriber, ReplyUri=memory://subscriber2
``` Registry
HasGlobalSubscriptionsRegistry
```

|> LoadNode Key=Subscriber, ReplyUri=memory://subscriber3
``` Registry
HasGlobalSubscriptionsRegistry
```

|> ForNode Key=Publisher
|> TheActiveSubscriptionsAre
    [rows]
    |> TheActiveSubscriptionsAre-row NodeName=Publishing
    ``` MessageType
    FubuMVC.IntegrationTesting.Fixtures.ServiceBus.Support.OneMessage
    ```

    ``` Source
    memory://harness/publisher1
    ```

    ``` Receiver
    memory://harness/subscriber1
    ```


|> ThePersistedSubscriptionsAre
    [rows]
    |> ThePersistedSubscriptionsAre-row NodeName=Publishing
    ``` MessageType
    FubuMVC.IntegrationTesting.Fixtures.ServiceBus.Support.OneMessage
    ```

    ``` Source
    memory://harness/publisher1
    ```

    ``` Receiver
    memory://harness/subscriber1
    ```


|> ThePersistedTransportNodesAre
    [rows]
    |> ThePersistedTransportNodesAre-row NodeName=Publishing, Address=memory://publisher1/

|> ForNode Key=Subscriber
|> ThePersistedTransportNodesAre
    [rows]
    |> ThePersistedTransportNodesAre-row NodeName=GlobalSubscriber, Address=memory://subscriber1/
    |> ThePersistedTransportNodesAre-row NodeName=GlobalSubscriber, Address=memory://subscriber2/
    |> ThePersistedTransportNodesAre-row NodeName=GlobalSubscriber, Address=memory://subscriber3/

~~~
