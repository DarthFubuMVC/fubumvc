# Sliding expiration success and failure

-> id = 59a6d354-2ce9-4255-b96d-82e075ee64bd
-> lifecycle = Regression
-> max-retries = 0
-> last-updated = 2017-02-23T15:45:01.0577952-06:00
-> tags = 

[Model]
|> UsersAre
    [table]
    |> row UserName=jeremy, Password=jasper

|> SetAuthenticationSettings ExpireInMinutes=60, SlidingExpiration=True, MaximumNumberOfFailedAttempts=3, CooloffPeriodInMinutes=60
~~~

[LoginScreen]
|> OpenLoginScreen
|> Login user=jeremy, password=jasper
|> IsNotOnTheLoginScreen

Well within the expiration time, go to the login screen again

|> AfterMinutes number=30
|> TryToGoHome
|> ShouldBeOnTheHomePage
|> AfterMinutes number=59
|> TryToGoHome
|> ShouldBeOnTheHomePage

Now, be idle for longer than the expiration time and try to refresh the home page.  You should be redirected to the login screen

|> AfterMinutes number=61
|> TryToGoHome
|> IsOnTheLoginScreen
~~~
