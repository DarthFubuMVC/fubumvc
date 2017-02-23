# Remove local subscriptions

-> id = d7be9475-d342-455c-9e8c-3c24563afb27
-> lifecycle = Regression
-> max-retries = 0
-> last-updated = 2017-02-23T15:45:01.2447952-06:00
-> tags = 

[Subscriptions]
|> LoadNode Key=Publisher, Registry=PublishingRegistry, ReplyUri=memory://publisher1
|> LoadNode Key=Subscriber, ReplyUri=memory://subscriber1
``` Registry
HasLocalSubscriptionsRegistry
```

|> LoadNode Key=SubscriberTwo, ReplyUri=memory://subscriber2
``` Registry
HasLocalSubscriptionsRegistry
```

|> NodeRemovesLocalSubscritpions Key=Subscriber
|> ForNode Key=Publisher
|> TheActiveSubscriptionsAre
    [rows]
    |NodeName  |MessageType                                                      |Source                     |Receiver            |
    |Publishing|FubuMVC.IntegrationTesting.Fixtures.ServiceBus.Support.OneMessage|memory://harness/publisher1|memory://subscriber2|
    |Publishing|FubuMVC.IntegrationTesting.Fixtures.ServiceBus.Support.TwoMessage|memory://harness/publisher1|memory://subscriber2|

|> ThePersistedSubscriptionsAre
    [rows]
    |NodeName  |MessageType                                                      |Source                     |Receiver            |
    |Publishing|FubuMVC.IntegrationTesting.Fixtures.ServiceBus.Support.OneMessage|memory://harness/publisher1|memory://subscriber2|
    |Publishing|FubuMVC.IntegrationTesting.Fixtures.ServiceBus.Support.TwoMessage|memory://harness/publisher1|memory://subscriber2|

|> ForNode Key=Subscriber

The other "LocalSubscriber" node will still have its subscriptions.

|> TheLocalSubscriptionsAre
    [rows]
    |NodeName       |MessageType                                                      |Source                     |Receiver            |
    |LocalSubscriber|FubuMVC.IntegrationTesting.Fixtures.ServiceBus.Support.OneMessage|memory://harness/publisher1|memory://subscriber2|
    |LocalSubscriber|FubuMVC.IntegrationTesting.Fixtures.ServiceBus.Support.TwoMessage|memory://harness/publisher1|memory://subscriber2|

~~~
