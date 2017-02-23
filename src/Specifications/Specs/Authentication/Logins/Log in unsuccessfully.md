# Log in unsuccessfully

-> id = c7b52565-416a-4fa2-9ef0-b9bdbed4b786
-> lifecycle = Regression
-> max-retries = 0
-> last-updated = 2017-02-23T15:45:01.0207952-06:00
-> tags = 

[Model]
|> UsersAre
    [table]
    |> row UserName=jeremy, Password=jasper

|> SetAuthenticationSettings ExpireInMinutes=180, SlidingExpiration=True, MaximumNumberOfFailedAttempts=3, CooloffPeriodInMinutes=60
~~~

[LoginScreen]
|> OpenLoginScreen
|> Login user=alba, password=wrong
|> TheMessageShouldBe
``` message
Incorrect credentials. Attempt 1 of 3
```

|> Login user=alba, password=wrong
|> TheMessageShouldBe
``` message
Incorrect credentials. Attempt 2 of 3
```


On the third attempt, we lock out the user until the cooling off period is done

|> Login user=alba, password=wrong
|> TheMessageShouldBe
``` message
This user is locked out because of too many failed login attempts. Try back later.
```

|> Login user=alba, password=wrong
|> TheMessageShouldBe
``` message
This user is locked out because of too many failed login attempts. Try back later.
```

~~~
