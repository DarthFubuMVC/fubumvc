# Login successfully to a different page than home

-> id = 20a7d4fe-bb85-4fdc-a211-1178f63638dd
-> lifecycle = Regression
-> max-retries = 0
-> last-updated = 2017-02-23T15:45:01.0327952-06:00
-> tags = 

[Model]
|> UsersAre
    [table]
    |> UsersAre-row UserName=jeremy, Password=jasper

~~~

[OtherScreen]
|> GoToDifferentPage name=jeremy
|> IsOnTheLoginScreen
|> Login user=jeremy, password=jasper
|> IsNotOnTheLoginScreen
|> ShouldBeOnTheDifferentPage name=jeremy
~~~
