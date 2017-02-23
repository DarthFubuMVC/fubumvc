# Publishing a message using publishing rules

-> id = 8b660d24-345e-48d2-9627-14ece71eb019
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

|> Actions
    [ServiceBusAction]
    |> Send
        [table]
        |> Send-row Key=One, Node=Website1, Message=OneMessage


|> MessagesProcessedShouldBe
    [rows]
    |> MessagesProcessedShouldBe-row Key=One, Node=Service1, Type=OneMessage

~~~
