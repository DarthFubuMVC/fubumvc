# Publishing a message that has multiple subscribers

-> id = f5da15b1-0d4c-4567-af40-dad3925eb89d
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
        |> Publishes-row Channel=Service3, Message=OneMessage


|> ActiveNode
    [ServiceBusNode]
    |> NodeName name=Service1, channels=Service1

|> ActiveNode
    [ServiceBusNode]
    |> NodeName name=Service2, channels=Service2

|> ActiveNode
    [ServiceBusNode]
    |> NodeName name=Service3, channels=Service3

|> Actions
    [ServiceBusAction]
    |> Send
        [table]
        |> Send-row Key=One, Node=Website1, Message=OneMessage


|> MessagesProcessedShouldBe
    [rows]
    |> MessagesProcessedShouldBe-row Key=One, Node=Service1, Type=OneMessage
    |> MessagesProcessedShouldBe-row Key=One, Node=Service3, Type=OneMessage

~~~
