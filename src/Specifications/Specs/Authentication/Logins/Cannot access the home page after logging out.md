# Cannot access the home page after logging out

-> id = 8fb49522-eb53-4c97-991c-0d6191712fee
-> lifecycle = Regression
-> max-retries = 0
-> last-updated = 2017-02-23T15:45:00.9427952-06:00
-> tags = 

[LoginScreen]

You should not be able to access anything but the login screen after logging out

|> Logout
|> TryToGoHome
|> IsOnTheLoginScreen
~~~
