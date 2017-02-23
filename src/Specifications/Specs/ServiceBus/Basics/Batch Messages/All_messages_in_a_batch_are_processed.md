# All messages in a batch are processed

-> id = db0cb182-83b7-406d-9466-7ac8c16be1d8
-> lifecycle = Regression
-> max-retries = 0
-> last-updated = 2017-02-23T15:45:01.1307952-06:00
-> tags = 

[BatchMessage]
|> SendBatch
    [DefineBatchMessage]
    |> DescriptionIs description=The batch
    |> SendColor color=Red
    |> SendDirection direction=North
    |> SendState state=Texas
    |> SendTeam team=Chiefs

|> TheRecordedMessagesAre
    [Rows]
    |expected            |
    |Starting: The batch |
    |Color: Red          |
    |Direction: North    |
    |State: Texas        |
    |Team: Chiefs        |
    |Finishing: The batch|

~~~
