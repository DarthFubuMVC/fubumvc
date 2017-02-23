# Audit user login successes and failures

-> id = d7f95ebd-0de7-4b28-a3fc-fc58d2bbfa7d
-> lifecycle = Regression
-> max-retries = 0
-> last-updated = 2017-02-23T15:45:01.0637952-06:00
-> tags = 

[Model]
|> UsersAre
    [table]
    |UserName|Password |
    |jeremy  |jasper   |
    |rand    |tworivers|

|> SetAuthenticationSettings ExpireInMinutes=180, SlidingExpiration=True, MaximumNumberOfFailedAttempts=3, CooloffPeriodInMinutes=60
~~~

[LoginScreen]
|> OpenLoginScreen

Fail once, then succeed for 'jeremy'

|> Login user=jeremy, password=alba
|> Login user=jeremy, password=jasper
|> Logout

Two failures, including one for a nonexistent user name

|> Login user=rand, password=andor
|> Login user=thom, password=andor

Successfully login as 'rand'

|> Login user=rand, password=tworivers
~~~

[LoginAuditing]
|> TheAuditsAre
    [rows]
    |Username|Type        |
    |jeremy  |LoginFailure|
    |jeremy  |LoginSuccess|
    |rand    |LoginFailure|
    |thom    |LoginFailure|
    |rand    |LoginSuccess|

~~~
