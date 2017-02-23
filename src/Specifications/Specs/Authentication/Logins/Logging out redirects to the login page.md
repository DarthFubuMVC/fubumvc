# Logging out redirects to the login page

-> id = 8a89b1f5-3d6b-4766-a5f5-985ff48c5034
-> lifecycle = Regression
-> max-retries = 0
-> last-updated = 2017-02-23T15:45:01.0257952-06:00
-> tags = 

[LoginScreen]

First, we're going to logout to make sure we're clean.  Loggin out should direct us to the login screen

|> Logout
|> IsOnTheLoginScreen
~~~
