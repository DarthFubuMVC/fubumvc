# Remember me

-> id = 5565d562-fa61-455f-90aa-abaf9ccea155
-> lifecycle = Acceptance
-> max-retries = 0
-> last-updated = 2017-02-23T15:45:01.0457952-06:00
-> tags = 


This test is unreliable because of the cookie state.  Despite my efforts at cleaning up the existing cookie state, this test is only trustworthy after recycling the environment.


[LoginScreen]
|> OpenLoginScreen
|> CheckRememberMe
|> Login user=jeremy, password=jasper
|> Logout
|> OpenLoginScreen
|> CheckUserName UserName=jeremy
|> CheckRememberMe
|> Login user=logistics, password=L0gistics!
|> Logout
|> CheckUserName UserName=logistics
~~~
