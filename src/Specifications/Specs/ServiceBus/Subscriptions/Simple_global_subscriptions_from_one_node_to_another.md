# Simple global subscriptions from one node to another

-> id = b6586525-f6a7-46c9-b230-8a12affb0c66
-> lifecycle = Regression
-> max-retries = 0
-> last-updated = 2017-02-23T15:45:01.2497952-06:00
-> tags = 

[Subscriptions]
|> LoadNode Key=Publisher, Registry=PublishingRegistry, ReplyUri=memory://publisher1
|> LoadNode Key=Subscriber, ReplyUri=memory://subscriber1
``` Registry
HasGlobalSubscriptionsRegistry
```

|> ForNode Key=Publisher
|> TheActiveSubscriptionsAre
    [rows]
    |> row NodeName=Publishing
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
    |> row NodeName=Publishing
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
    |> row NodeName=Publishing, Address=memory://publisher1/

|> ForNode Key=Subscriber
|> ThePersistedTransportNodesAre
    [rows]
    |> row NodeName=GlobalSubscriber, Address=memory://subscriber1/

~~~
