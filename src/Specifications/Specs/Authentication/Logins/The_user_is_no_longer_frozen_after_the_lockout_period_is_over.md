# The user is no longer frozen after the lockout period is over

-> id = 2cdcf7c0-4d3a-4edd-87c3-8a7ab6dd3011
-> lifecycle = Regression
-> max-retries = 0
-> last-updated = 2017-02-23T15:45:01.0527952-06:00
-> tags = 

[Model]
|> SetAuthenticationSettings ExpireInMinutes=180, SlidingExpiration=True, MaximumNumberOfFailedAttempts=2, CooloffPeriodInMinutes=60
|> UsersAre
    [table]
    |> UsersAre-row UserName=bob, Password=pace

~~~

[LoginScreen]
|> OpenLoginScreen
|> Login user=bob, password=wrong
|> Login user=bob, password=wrong
|> TheLockedOutMessageShouldBeDisplayed

If the user is no longer locked out, we should no longer see the locked out message when we first open the screen

|> ReopenTheLoginScreen number=70
|> NoMessageIsShown
|> Login user=bob, password=pace
|> IsNotOnTheLoginScreen
~~~
