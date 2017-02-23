# Simple local subscription

-> id = 23cb7ab9-8cf7-495f-948d-74d6f25ea26b
-> lifecycle = Regression
-> max-retries = 0
-> last-updated = 2015-09-11T00:00:00.0000000
-> tags = 

[Subscriptions]
|> LoadNode Key=Publisher, Registry=PublishingRegistry, ReplyUri=memory://publisher1
|> LoadNode Key=Subscriber, ReplyUri=memory://subscriber1
``` Registry
HasLocalSubscriptionsRegistry
```

|> ForNode Key=Publisher
|> TheActiveSubscriptionsAre
    [rows]
    |> TheActiveSubscriptionsAre-row NodeName=Publishing, Receiver=memory://subscriber1
    ``` MessageType
    FubuMVC.IntegrationTesting.Fixtures.ServiceBus.Support.OneMessage
    ```

    ``` Source
    memory://harness/publisher1
    ```

    |> TheActiveSubscriptionsAre-row NodeName=Publishing, Receiver=memory://subscriber1
    ``` MessageType
    FubuMVC.IntegrationTesting.Fixtures.ServiceBus.Support.TwoMessage
    ```

    ``` Source
    memory://harness/publisher1
    ```


|> ThePersistedSubscriptionsAre
    [rows]
    |> ThePersistedSubscriptionsAre-row NodeName=Publishing, Receiver=memory://subscriber1
    ``` MessageType
    FubuMVC.IntegrationTesting.Fixtures.ServiceBus.Support.OneMessage
    ```

    ``` Source
    memory://harness/publisher1
    ```

    |> ThePersistedSubscriptionsAre-row NodeName=Publishing, Receiver=memory://subscriber1
    ``` MessageType
    FubuMVC.IntegrationTesting.Fixtures.ServiceBus.Support.TwoMessage
    ```

    ``` Source
    memory://harness/publisher1
    ```


|> LoadNode Key=SubscriberTwo, ReplyUri=memory://subscriber2
``` Registry
HasLocalSubscriptionsRegistry
```

|> ForNode Key=Publisher
|> TheActiveSubscriptionsAre
    [rows]
    |> TheActiveSubscriptionsAre-row NodeName=Publishing, Receiver=memory://subscriber1
    ``` MessageType
    FubuMVC.IntegrationTesting.Fixtures.ServiceBus.Support.OneMessage
    ```

    ``` Source
    memory://harness/publisher1
    ```

    |> TheActiveSubscriptionsAre-row NodeName=Publishing, Receiver=memory://subscriber1
    ``` MessageType
    FubuMVC.IntegrationTesting.Fixtures.ServiceBus.Support.TwoMessage
    ```

    ``` Source
    memory://harness/publisher1
    ```

    |> TheActiveSubscriptionsAre-row NodeName=Publishing, Receiver=memory://subscriber2
    ``` MessageType
    FubuMVC.IntegrationTesting.Fixtures.ServiceBus.Support.OneMessage
    ```

    ``` Source
    memory://harness/publisher1
    ```

    |> TheActiveSubscriptionsAre-row NodeName=Publishing, Receiver=memory://subscriber2
    ``` MessageType
    FubuMVC.IntegrationTesting.Fixtures.ServiceBus.Support.TwoMessage
    ```

    ``` Source
    memory://harness/publisher1
    ```


|> ThePersistedSubscriptionsAre
    [rows]
    |> ThePersistedSubscriptionsAre-row NodeName=Publishing, Receiver=memory://subscriber1
    ``` MessageType
    FubuMVC.IntegrationTesting.Fixtures.ServiceBus.Support.OneMessage
    ```

    ``` Source
    memory://harness/publisher1
    ```

    |> ThePersistedSubscriptionsAre-row NodeName=Publishing, Receiver=memory://subscriber1
    ``` MessageType
    FubuMVC.IntegrationTesting.Fixtures.ServiceBus.Support.TwoMessage
    ```

    ``` Source
    memory://harness/publisher1
    ```

    |> ThePersistedSubscriptionsAre-row NodeName=Publishing, Receiver=memory://subscriber2
    ``` MessageType
    FubuMVC.IntegrationTesting.Fixtures.ServiceBus.Support.OneMessage
    ```

    ``` Source
    memory://harness/publisher1
    ```

    |> ThePersistedSubscriptionsAre-row NodeName=Publishing, Receiver=memory://subscriber2
    ``` MessageType
    FubuMVC.IntegrationTesting.Fixtures.ServiceBus.Support.TwoMessage
    ```

    ``` Source
    memory://harness/publisher1
    ```


~~~
