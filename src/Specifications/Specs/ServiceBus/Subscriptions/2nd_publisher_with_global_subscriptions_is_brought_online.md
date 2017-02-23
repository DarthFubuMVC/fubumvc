# 2nd publisher with global subscriptions is brought online

-> id = 2349b85e-1ff3-492a-891f-4ec93a19cc9b
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

|> LoadNode Key=PublisherTwo, Registry=PublishingRegistry, ReplyUri=memory://publisher2
|> ForNode Key=PublisherTwo
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
    |> ThePersistedTransportNodesAre-row NodeName=Publishing, Address=memory://publisher2/

~~~
