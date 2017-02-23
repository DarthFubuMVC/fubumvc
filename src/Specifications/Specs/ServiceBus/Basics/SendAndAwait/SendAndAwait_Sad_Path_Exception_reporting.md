# SendAndAwait Sad Path Exception reporting

-> id = 74248f7d-bfa6-49d0-9858-3423b0dad6b2
-> lifecycle = Regression
-> max-retries = 0
-> last-updated = 2017-02-23T15:45:01.1437952-06:00
-> tags = 

[SendAndAwait]
|> SendMessageUnsuccessfully
|> AckIsReceived seconds=10
|> TheAckFailedWithMessage message=AmbiguousMatchException
~~~
