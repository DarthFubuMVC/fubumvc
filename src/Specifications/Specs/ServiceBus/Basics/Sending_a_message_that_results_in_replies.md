# Sending a message that results in replies

-> id = 737fa168-2876-414d-9537-514fa4cd6748
-> lifecycle = Regression
-> max-retries = 0
-> last-updated = 2015-09-23T00:00:00.0000000
-> tags = 

[ServiceBus]
|> ActiveNode
    [ServiceBusNode]
    |> NodeName name=Website1, channels=Website1
    |> Publishes
        [table]
        |> Publishes-row Channel=Service1, Message=OneMessage


|> ActiveNode
    [ServiceBusNode]
    |> NodeName name=Service1, channels=Service1
    |> Publishes
        [table]
        |> Publishes-row Channel=Service1, Message=TwoMessage
        |> Publishes-row Channel=Service1, Message=ThreeMessage

    |> Replies
        [table]
        |> Replies-row Message=OneMessage, Reply=TwoMessage
        |> Replies-row Message=OneMessage, Reply=ThreeMessage


|> Actions
    [ServiceBusAction]
    |> Send
        [table]
        |> Send-row Key=One, Node=Website1, Message=OneMessage


|> MessagesProcessedShouldBe
    [rows]
    |> MessagesProcessedShouldBe-row Key=One, Node=Service1, Type=OneMessage
    |> MessagesProcessedShouldBe-row Key=One, Node=Service1, Type=TwoMessage
    |> MessagesProcessedShouldBe-row Key=One, Node=Service1, Type=ThreeMessage

~~~
