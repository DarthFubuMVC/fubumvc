# Multiple publishers all get new subscriptions

-> id = ee67cd60-cdec-4d31-8db1-26938175fe49
-> lifecycle = Acceptance
-> max-retries = 0
-> last-updated = 2015-09-11T00:00:00.0000000
-> tags = 

[Subscriptions]
|> LoadNode Key=Publisher, Registry=PublishingRegistry, ReplyUri=memory://publisher1
|> LoadNode Key=PublisherTwo, Registry=PublishingRegistry, ReplyUri=memory://publisher2
|> LoadNode Key=PublisherThree, Registry=PublishingRegistry, ReplyUri=memory://publisher3
|> LoadNode Key=Subscriber, ReplyUri=memory://subscriber1
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


|> ForNode Key=PublisherThree
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


~~~
